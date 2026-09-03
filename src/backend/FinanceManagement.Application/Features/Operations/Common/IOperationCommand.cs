using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Application.Features.Operations.Common;

public interface IOperationCommand
{
    Guid TypeId { get; }
    Guid WalletId { get; }
    decimal Amount { get; }
    DateTime Date { get; }
    string? Note { get; }
    Currency? Currency { get; }
}
