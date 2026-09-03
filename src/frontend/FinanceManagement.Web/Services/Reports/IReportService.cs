using FinanceManagement.Contracts.Enums;
using FinanceManagement.Contracts.Reports;

namespace FinanceManagement.Web.Services.Reports;

public interface IReportService
{
    Task<ReportResponse> GetDailyAsync(DateTime date, Currency currency, CancellationToken ct = default);
    Task<ReportResponse> GetPeriodAsync(DateTime from, DateTime to, Currency currency, CancellationToken ct = default);
}
