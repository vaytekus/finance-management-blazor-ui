using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.Features.Wallets.Queries.GetAllWallets;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Domain.Common;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Infrastructure.Data;
using FinanceManagement.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Repositories;

public class WalletRepository(AppDbContext db) : IWalletRepository
{

    public async Task<IReadOnlyList<Wallet>> GetAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        return await db.Wallets.Where(w => w.UserId == userId)
            .OrderBy(w => w.CreatedAt)
            .ToListAsync(ct);
    }
    
    public Task<Wallet?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        return db.Wallets
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, ct);
    }
    
    public async Task<bool> ExistsByNameForUserAsync(string name, Guid userId, Guid? excludeId, CancellationToken ct = default)
    {
        return await db.Wallets
            .AnyAsync(w => 
                w.Name == name 
                && w.UserId == userId
                && (excludeId == null || w.Id != excludeId), ct);
    }

    public Task<bool> HasOperationsAsync(Guid walletId, CancellationToken ct = default)
    {
        return db.Operations.AnyAsync(w => w.WalletId == walletId, ct);
    }

    public void Add(Wallet wallet)
    {
        db.Wallets.Add(wallet);
    }
    
    public void Update(Wallet wallet)
    {
        db.Wallets.Update(wallet);
    }
    
    public void SoftDelete(Wallet wallet)
    {
        wallet.MarkDeleted();
        db.Wallets.Update(wallet);
    }
    public async Task<PagedResult<Wallet>> GetPagedForUserAsync(
        Guid userId, 
        GetAllWalletsQuery query, 
        CancellationToken ct = default)
    {
        var q = db.Wallets.Where(w => w.UserId == userId);

        if (query.Currency.HasValue)
        {
            q = q.Where(w => w.Currency == query.Currency.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            q = q.Where(w => EF.Functions.ILike(w.Name, $"%{query.Search}%"));
        }

        q = (query.SortBy?.ToLowerInvariant(), query.SortOrder) switch
        {
            ("name", SortOrder.Asc) => q.OrderBy(w => w.Name),
            ("name", SortOrder.Desc) => q.OrderByDescending(w => w.Name),
            ("currency", SortOrder.Asc) => q.OrderBy(w => w.Currency),
            ("currency", SortOrder.Desc) => q.OrderByDescending(w => w.Currency),
            ("createdat", SortOrder.Desc) => q.OrderByDescending(w => w.CreatedAt),
            (_, SortOrder.Desc) => q.OrderByDescending(w => w.CreatedAt),
            _ => q.OrderBy(w => w.CreatedAt)
        };

        return await q.ToPagedResultAsync(query.Page, query.PageSize, ct);
    }
}
