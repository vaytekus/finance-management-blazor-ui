using FinanceManagement.Contracts.Reports;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Reports.Queries.GetPeriodReport;

public record GetPeriodReportQuery(DateTime From, DateTime To, Currency Currency = Currency.UAH) : IRequest<ReportResponse>;
