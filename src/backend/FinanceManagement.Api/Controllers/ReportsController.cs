using FinanceManagement.Contracts.Reports;
using FinanceManagement.Application.Features.Reports.Queries.GetDailyReport;
using FinanceManagement.Application.Features.Reports.Queries.GetPeriodReport;
using FinanceManagement.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("daily")]
    public async Task<ActionResult<ReportResponse>> GetDailyReport(
        [FromQuery] DateTime date,
        [FromQuery] Currency currency = Currency.UAH,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetDailyReportQuery(date, currency), ct));

    [HttpGet("period")]
    public async Task<ActionResult<ReportResponse>> GetPeriodReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] Currency currency = Currency.UAH,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetPeriodReportQuery(from, to, currency), ct));
}
