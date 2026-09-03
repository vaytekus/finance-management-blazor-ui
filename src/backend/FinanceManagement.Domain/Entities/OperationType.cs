using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Domain.Entities;

public class OperationType
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public OperationKind Kind { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
