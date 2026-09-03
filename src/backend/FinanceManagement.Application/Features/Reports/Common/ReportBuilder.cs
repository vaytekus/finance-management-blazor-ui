using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using FinanceManagement.Contracts.Reports;
using ContractsEnums = FinanceManagement.Contracts.Enums;
using DomainEnums = FinanceManagement.Domain.Enums;

namespace FinanceManagement.Application.Features.Reports.Common;

public class ReportBuilder
{
    private readonly IReportRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly ICurrencyConverter _converter;

    public ReportBuilder(
        IReportRepository repository,
        ICurrentUser currentUser,
        ICurrencyConverter converter)
    {
        _repository = repository;
        _currentUser = currentUser;
        _converter = converter;
    }

    public async Task<ReportResponse> BuildAsync(DateTime from, DateTime to, DomainEnums.Currency targetCurrency, CancellationToken ct)
    {
        var aggregates = await _repository.GetByTypeAsync(_currentUser.Id, from, to, ct);
        var operations = await _repository.GetOperationsAsync(_currentUser.Id, from, to, ct);

        foreach (var agg in aggregates)
        {
            agg.Total = await _converter.ConvertAsync(agg.Total, (DomainEnums.Currency)agg.Currency, targetCurrency, ct: ct);
        }

        var byType = aggregates
            .GroupBy(a => new { a.TypeId, a.TypeName, a.Kind })
            .Select(g => new ReportByItem
            {
                TypeId = g.Key.TypeId,
                TypeName = g.Key.TypeName,
                Kind = g.Key.Kind,
                Total = g.Sum(a => a.Total),
                OperationCount = g.Sum(a => a.OperationCount)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        var totalIncome = byType
            .Where(x => x.Kind == ContractsEnums.OperationKind.Income)
            .Sum(x => x.Total);

        var totalExpense = byType
            .Where(x => x.Kind == ContractsEnums.OperationKind.Expense)
            .Sum(x => x.Total);

        return new ReportResponse
        {
            From = from,
            To = to,
            Currency = (ContractsEnums.Currency)targetCurrency,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TotalBalance = totalIncome - totalExpense,
            ByType = byType,
            Operations = operations.Select(o => o.ToResponse()).ToList()
        };
    }

    public static DateTime StartOfDayUtc(DateTime date)
        => DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

    public static DateTime EndOfDayUtc(DateTime date)
        => DateTime.SpecifyKind(date.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
}
