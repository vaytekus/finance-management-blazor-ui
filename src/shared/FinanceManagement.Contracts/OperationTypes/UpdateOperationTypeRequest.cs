using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.OperationTypes;

public class UpdateOperationTypeRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public OperationKind Kind { get; set; }
}
