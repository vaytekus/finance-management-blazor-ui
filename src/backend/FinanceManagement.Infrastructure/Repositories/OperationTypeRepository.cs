using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.Features.OperationTypes.Queries.GetAllOperationTypes;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Infrastructure.Data;
using FinanceManagement.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Repositories;

public class OperationTypeRepository : IOperationTypeRepository
{
    private readonly AppDbContext _db;

    public OperationTypeRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<IReadOnlyList<OperationType>> GetAllForUserAsync(Guid userId, CancellationToken ct = default) 
    {
        return await _db.OperationTypes
            .Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync(ct);
    }
    
    public Task<OperationType?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default) 
    {
        return _db.OperationTypes
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);
    }
    
    public Task<bool> ExistsByNameForUserAsync(string name, Guid userId, Guid? excludeId = null, CancellationToken ct = default)
    {
        return _db.OperationTypes
            .AnyAsync(x => 
                x.Name == name
                && x.UserId == userId
                && (excludeId == null || x.Id != excludeId), ct);
    }
    
    public Task<bool> IsUsedInOperationsAsync(Guid typeId, CancellationToken ct = default) 
    {
        return _db.Operations
            .AnyAsync(x => x.TypeId == typeId, ct);
    }
    public async Task<int> CountOperationsAsync(Guid typeId, CancellationToken ct = default)
    {
        return await _db.Operations
            .CountAsync(o => o.TypeId == typeId);
    }

    public async Task ReassignOperationsAsync(Guid fromTypeId, Guid toTypeId, CancellationToken ct = default)
    {
        await _db.Operations
            .Where(o => o.TypeId == fromTypeId)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.TypeId, toTypeId), ct);
    }

    public void Add(OperationType entity) 
    {
        _db.OperationTypes.Add(entity);
    }
    
    public void Update(OperationType entity) 
    {
        _db.OperationTypes.Update(entity);
    }
    
    public void Delete(OperationType entity) 
    {
        _db.OperationTypes.Remove(entity);
    }
    public async Task<PagedResult<OperationType>> GetPagedForUserAsync(
        Guid userId, 
        GetAllOperationTypesQuery query, 
        CancellationToken ct = default)
    {
        var q = _db.OperationTypes.Where(x => x.UserId == userId);

        if (query.Kind.HasValue)
        {
            q = q.Where(x => x.Kind == query.Kind);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search}%";

            q = q.Where(x =>
                EF.Functions.ILike(x.Name, pattern) ||
                (x.Description != null && EF.Functions.ILike(x.Description, pattern)));
        }

        q = (query.SortBy?.ToLowerInvariant(), query.SortOrder) switch
        {
            ("kind", SortOrder.Asc) => q.OrderBy(x => x.Kind),
            ("kind", SortOrder.Desc) => q.OrderByDescending(x => x.Kind),
            ("name", SortOrder.Desc) => q.OrderByDescending(x => x.Name),
            (_, SortOrder.Desc) => q.OrderByDescending(x => x.Name),
            _ => q.OrderBy(x => x.Name),
        };

        return await q.ToPagedResultAsync(query.Page, query.PageSize, ct);
    }

}
