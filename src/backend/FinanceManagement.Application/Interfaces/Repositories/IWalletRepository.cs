using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.Features.Wallets.Queries.GetAllWallets;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Repositories;

public interface IWalletRepository : IUserScopedRepository<Wallet>
{
    Task<bool> ExistsByNameForUserAsync(string name, Guid userId, Guid? excludeId, CancellationToken ct = default);
    Task<bool> HasOperationsAsync(Guid walletId, CancellationToken ct = default);

    void SoftDelete(Wallet wallet);
    
    Task<PagedResult<Wallet>> GetPagedForUserAsync(
        Guid userId,
        GetAllWalletsQuery query,
        CancellationToken ct = default);
}
