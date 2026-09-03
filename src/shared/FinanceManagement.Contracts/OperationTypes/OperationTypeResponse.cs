using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.OperationTypes;

public class OperationTypeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public OperationKind Kind { get; set; }
}
