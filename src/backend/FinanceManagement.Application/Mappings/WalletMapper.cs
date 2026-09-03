using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Mappings;

public static class WalletMapper
{
    public static WalletResponse ToResponse(this Wallet wallet) => new()
    {
        Id = wallet.Id,
        Name = wallet.Name,
        Currency = (Contracts.Enums.Currency)wallet.Currency,
        CreatedAt = wallet.CreatedAt,
    };
}
