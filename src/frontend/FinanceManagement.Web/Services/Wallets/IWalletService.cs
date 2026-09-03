using FinanceManagement.Contracts.Wallets;

namespace FinanceManagement.Web.Services.Wallets;

public interface IWalletService
{
    Task<IReadOnlyList<WalletResponse>> GetAllAsync(CancellationToken ct = default);
    Task<WalletResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<WalletResponse> CreateAsync(CreateWalletRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateWalletRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
