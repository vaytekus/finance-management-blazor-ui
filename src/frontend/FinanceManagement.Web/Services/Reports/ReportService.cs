using System.Net.Http.Json;
using FinanceManagement.Contracts.Enums;
using FinanceManagement.Contracts.Reports;
using FinanceManagement.Web.Services.Common;

namespace FinanceManagement.Web.Services.Reports;

public class ReportService : IReportService
{
    private const string _basePath = "api/reports";

    private readonly HttpClient _http;

    public ReportService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ReportResponse> GetDailyAsync(DateTime date, Currency currency, CancellationToken ct = default)
    {
        var url = $"{_basePath}/daily?date={Uri.EscapeDataString(date.ToString("O"))}&currency={currency}";
        var result = await _http.GetFromJsonAsync<ReportResponse>(url, ApiJsonOptions.Default, ct);
        return result ?? throw new InvalidOperationException("Empty response from GET /api/reports/daily");
    }

    public async Task<ReportResponse> GetPeriodAsync(DateTime from, DateTime to, Currency currency, CancellationToken ct = default)
    {
        var url = $"{_basePath}/period?from={Uri.EscapeDataString(from.ToString("O"))}&to={Uri.EscapeDataString(to.ToString("O"))}&currency={currency}";
        var result = await _http.GetFromJsonAsync<ReportResponse>(url, ApiJsonOptions.Default, ct);
        return result ?? throw new InvalidOperationException("Empty response from GET /api/reports/period");
    }
}
