using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.Wallets;

public class CreateWalletRequest
{
    public string Name { get; set; } = null!;
    public Currency Currency { get; set; }
}
