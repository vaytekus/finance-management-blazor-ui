using FinanceManagement.Contracts.Reports;
using FinanceManagement.Application.Features.Reports.Common;
using MediatR;

namespace FinanceManagement.Application.Features.Reports.Queries.GetDailyReport;

public class GetDailyReportQueryHandler : IRequestHandler<GetDailyReportQuery, ReportResponse>
{
    private readonly ReportBuilder _builder;

    public GetDailyReportQueryHandler(ReportBuilder builder)
    {
        _builder = builder;
    }

    public Task<ReportResponse> Handle(GetDailyReportQuery request, CancellationToken ct)
    {
        var from = ReportBuilder.StartOfDayUtc(request.Date);
        var to = ReportBuilder.EndOfDayUtc(request.Date);
        return _builder.BuildAsync(from, to, request.Currency, ct);
    }
}
