using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.Operations;

public class UpdateOperationRequest
{
    public Guid TypeId { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public Currency? Currency { get; set; }
}
