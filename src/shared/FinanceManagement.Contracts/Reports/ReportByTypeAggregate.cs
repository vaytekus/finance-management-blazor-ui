using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.Reports;

public class ReportByTypeAggregate
{
    public Guid TypeId { get; set; }
    public string TypeName { get; set; } = null!;
    public OperationKind Kind { get; set; }
    public Currency Currency { get; set; }
    public decimal Total { get; set; }
    public int OperationCount { get; set; }
}
