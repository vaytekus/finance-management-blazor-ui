using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.Wallets;

public class UpdateWalletRequest
{
    public string Name { get; set; } = null!;
    public Currency Currency { get; set; }
}
