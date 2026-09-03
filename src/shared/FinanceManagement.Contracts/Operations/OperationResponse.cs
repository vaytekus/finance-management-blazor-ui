using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.Operations;

public class OperationResponse
{
    public Guid Id { get; set; }
    public Guid TypeId { get; set; }
    public string TypeName { get; set; } = null!;
    public OperationKind Kind { get; set; }
    public Guid WalletId { get; set; }
    public string WalletName { get; set; } = null!;
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
}
