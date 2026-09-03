using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.Wallets;

public class WalletResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Currency Currency { get; set; }
    public DateTime CreatedAt { get; set; }
}
