using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Contracts.Reports;

public class ReportByItem
{
    public Guid TypeId { get; set; }
    public string TypeName { get; set; } = null!;
    public OperationKind Kind { get; set; }
    public decimal Total { get; set; }
    public int OperationCount { get; set; }
}
