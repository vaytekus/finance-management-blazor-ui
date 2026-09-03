using FinanceManagement.Domain.Common;

namespace FinanceManagement.Domain.Entities;

public class Operation : ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid TypeId { get; set; }
    public OperationType? Type { get; set; }
    public Guid WalletId { get; set; }
    public Wallet? Wallet { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public DateTime? DeletedAt { get; set; }
}
