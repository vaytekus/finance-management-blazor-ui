using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Application.Common;

public record OperationTypeTemplate(string Name, OperationKind Kind)
{
    public OperationType ToEntity(Guid userId) => new()
    {
        Name = Name,
        Kind = Kind,
        UserId = userId
    };
}

public static class DefaultOperationTypes
{
    public static readonly IReadOnlyList<OperationTypeTemplate> All =
    [
        new("Salary",    OperationKind.Income),
        new("Freelance", OperationKind.Income),
        new("Food",      OperationKind.Expense),
        new("Transport", OperationKind.Expense),
        new("Rent",      OperationKind.Expense),
        new("Utilities", OperationKind.Expense),
        new("Health",    OperationKind.Expense),
        new("Other",     OperationKind.Expense)
    ];
}
