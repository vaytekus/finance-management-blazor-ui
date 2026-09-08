using FinanceManagement.Contracts.Reports;
using FinanceManagement.Application.Features.Reports.Common;
using MediatR;

namespace FinanceManagement.Application.Features.Reports.Queries.GetPeriodReport;

public class GetPeriodReportQueryHandler(ReportBuilder builder) : IRequestHandler<GetPeriodReportQuery, ReportResponse>
{
    public Task<ReportResponse> Handle(GetPeriodReportQuery request, CancellationToken ct)
    {
        var from = ReportBuilder.StartOfDayUtc(request.From);
        var to = ReportBuilder.EndOfDayUtc(request.To);
        return builder.BuildAsync(from, to, request.Currency, ct);
    }
}
