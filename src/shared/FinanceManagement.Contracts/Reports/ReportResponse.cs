using FinanceManagement.Contracts.Enums;
using FinanceManagement.Contracts.Operations;

namespace FinanceManagement.Contracts.Reports;

public class ReportResponse
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public Currency Currency { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal TotalBalance { get; set; }
    public IReadOnlyList<ReportByItem> ByType { get; set; } = [];
    public IReadOnlyList<OperationResponse> Operations { get; set; } = [];
}
