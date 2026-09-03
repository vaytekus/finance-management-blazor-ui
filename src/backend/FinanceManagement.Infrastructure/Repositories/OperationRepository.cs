using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.Features.Operations.Queries.GetAllOperations;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Domain.Common;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Infrastructure.Data;
using FinanceManagement.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Repositories;

public class OperationRepository : IOperationRepository
{
    private readonly AppDbContext _db;

    public OperationRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<IReadOnlyList<Operation>> GetAllForUserAsync(Guid userId, CancellationToken ct = default) 
    {
        return await _db.Operations
            .Include(x => x.Type)
            .Include(x => x.Wallet)
            .Where(x => x.Wallet!.UserId == userId)
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .ToListAsync(ct);
    }
    
    public async Task<Operation?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default) 
    {
        return await _db.Operations
            .Include(x => x.Type)
            .Include(x => x.Wallet)
            .FirstOrDefaultAsync(x => x.Id == id && x.Wallet!.UserId == userId, ct);
    }
    
    public void Add(Operation entity)
    {
        _db.Operations.Add(entity);
    }
    
    public void Update(Operation entity)
    {
        _db.Operations.Update(entity);
    }
    
    public void SoftDelete(Operation entity)
    {
        entity.MarkDeleted();
        _db.Operations.Update(entity);
    }
    public async Task<PagedResult<Operation>> GetPagedForUserAsync(
        Guid userId, 
        GetAllOperationsQuery query, 
        CancellationToken ct = default)
    {
        var q = _db.Operations
            .Include(x => x.Type)
            .Include(x => x.Wallet)
            .Where(x => x.Wallet!.UserId == userId);

        if (query.DateFrom.HasValue)
        {
            q = q.Where(x => x.Date >= DateTime.SpecifyKind(query.DateFrom.Value, DateTimeKind.Utc));
        }
        
        if (query.DateTo.HasValue)
        {
            q = q.Where(x => x.Date <= DateTime.SpecifyKind(query.DateTo.Value, DateTimeKind.Utc));
        }
        
        if (query.WalletId.HasValue)
        {
            q = q.Where(x => x.WalletId == query.WalletId.Value);
        }
        
        if (query.TypeId.HasValue)
        {
            q = q.Where(x => x.TypeId == query.TypeId.Value);
        }
        
        if (query.AmountMin.HasValue)
        {
            q = q.Where(x => x.Amount >= query.AmountMin.Value);
        }
        
        if (query.AmountMax.HasValue)
        {
            q = q.Where(x => x.Amount <= query.AmountMax.Value);
        }

        if (query.Currency.HasValue)
        {
            q = q.Where(x => x.Wallet!.Currency == query.Currency.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            q = q.Where(x => EF.Functions.ILike(x.Note!, $"%{query.Search}%"));
        }

        q = (query.SortBy?.ToLowerInvariant(), query.SortOrder) switch
        {
            ("amount", SortOrder.Asc) => q.OrderBy(x => x.Amount),
            ("amount", SortOrder.Desc) => q.OrderByDescending(x => x.Amount),
            ("note", SortOrder.Asc) => q.OrderBy(x => x.Note),
            ("note", SortOrder.Desc) => q.OrderByDescending(x => x.Note),
            (_, SortOrder.Asc) => q.OrderBy(x => x.Date),
            _ => q.OrderByDescending(x => x.Date)
        };

        return await q.ToPagedResultAsync(query.Page, query.PageSize, ct);
    }
}
