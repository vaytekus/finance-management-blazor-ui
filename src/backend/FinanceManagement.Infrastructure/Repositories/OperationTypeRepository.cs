using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.Features.OperationTypes.Queries.GetAllOperationTypes;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Infrastructure.Data;
using FinanceManagement.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Repositories;

public class OperationTypeRepository(AppDbContext db) : IOperationTypeRepository
{

    public async Task<IReadOnlyList<OperationType>> GetAllForUserAsync(Guid userId, CancellationToken ct = default) 
    {
        return await db.OperationTypes
            .Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync(ct);
    }
    
    public Task<OperationType?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default) 
    {
        return db.OperationTypes
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);
    }
    
    public Task<bool> ExistsByNameForUserAsync(string name, Guid userId, Guid? excludeId = null, CancellationToken ct = default)
    {
        return db.OperationTypes
            .AnyAsync(x => 
                x.Name == name
                && x.UserId == userId
                && (excludeId == null || x.Id != excludeId), ct);
    }
    
    public Task<bool> IsUsedInOperationsAsync(Guid typeId, CancellationToken ct = default) 
    {
        return db.Operations
            .AnyAsync(x => x.TypeId == typeId, ct);
    }
    public async Task<int> CountOperationsAsync(Guid typeId, CancellationToken ct = default)
    {
        return await db.Operations
            .CountAsync(o => o.TypeId == typeId);
    }

    public async Task ReassignOperationsAsync(Guid fromTypeId, Guid toTypeId, CancellationToken ct = default)
    {
        await db.Operations
            .Where(o => o.TypeId == fromTypeId)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.TypeId, toTypeId), ct);
    }

    public void Add(OperationType entity) 
    {
        db.OperationTypes.Add(entity);
    }
    
    public void Update(OperationType entity) 
    {
        db.OperationTypes.Update(entity);
    }
    
    public void Delete(OperationType entity) 
    {
        db.OperationTypes.Remove(entity);
    }
    public async Task<PagedResult<OperationType>> GetPagedForUserAsync(
        Guid userId, 
        GetAllOperationTypesQuery query, 
        CancellationToken ct = default)
    {
        var q = db.OperationTypes.Where(x => x.UserId == userId);

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
