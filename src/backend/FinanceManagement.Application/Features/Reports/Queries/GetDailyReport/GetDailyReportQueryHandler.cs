using FinanceManagement.Contracts.Reports;
using FinanceManagement.Application.Features.Reports.Common;
using MediatR;

namespace FinanceManagement.Application.Features.Reports.Queries.GetDailyReport;

public class GetDailyReportQueryHandler(ReportBuilder builder) : IRequestHandler<GetDailyReportQuery, ReportResponse>
{
    public Task<ReportResponse> Handle(GetDailyReportQuery request, CancellationToken ct)
    {
        var from = ReportBuilder.StartOfDayUtc(request.Date);
        var to = ReportBuilder.EndOfDayUtc(request.Date);
        return builder.BuildAsync(from, to, request.Currency, ct);
    }
}
