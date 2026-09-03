using FinanceManagement.Contracts.Reports;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Reports.Queries.GetDailyReport;

public record GetDailyReportQuery(DateTime Date, Currency Currency = Currency.UAH) : IRequest<ReportResponse>;
