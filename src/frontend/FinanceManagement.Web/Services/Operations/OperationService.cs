using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Web.Services.Common;
using System.Globalization;
using FinanceManagement.Web.Common;

namespace FinanceManagement.Web.Services.Operations;

public class OperationService(HttpClient http) : IOperationService
{
    private const string _basePath = "api/operations";

    public async Task<PagedResult<OperationResponse>> GetAsync(OperationQuery query, CancellationToken ct = default)
    {
        var url = BuildUrl(query);

        var result = await http.GetFromJsonAsync<PagedResult<OperationResponse>>(url, ApiJsonOptions.Default, ct);
        return result ?? new PagedResult<OperationResponse>();
    }

    public async Task<OperationResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{_basePath}/{id}", ct);
        return await response.ReadOrNullAsync<OperationResponse>(ct);
    }

    public async Task<OperationResponse> CreateAsync(CreateOperationRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(_basePath, request, ApiJsonOptions.Default, ct);
        return await response.ReadRequiredAsync<OperationResponse>(ct);
    }

    public async Task UpdateAsync(Guid id, UpdateOperationRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{_basePath}/{id}", request, ApiJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"{_basePath}/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    private static string BuildUrl(OperationQuery query)
    {
        var parts = new List<string>
        {
            $"Page={query.Page}",
            $"PageSize={query.PageSize}",
        };

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            parts.Add($"SortBy={Uri.EscapeDataString(query.SortBy)}");
            parts.Add($"SortOrder={(query.SortDescending ? "Desc" : "Asc")}");
        }

        if (query.DateFrom.HasValue)
        {
            parts.Add($"DateFrom={Uri.EscapeDataString(query.DateFrom.Value.ToString("O"))}");
        }

        if (query.DateTo.HasValue)
        {
            parts.Add($"DateTo={Uri.EscapeDataString(query.DateTo.Value.ToString("O"))}");
        }

        if (query.WalletId.HasValue)
        {
            parts.Add($"WalletId={query.WalletId.Value}");
        }

        if (query.TypeId.HasValue)
        {
            parts.Add($"TypeId={query.TypeId.Value}");
        }

        if (query.AmountMin.HasValue)
        {
            parts.Add($"AmountMin={query.AmountMin.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (query.AmountMax.HasValue)
        {
            parts.Add($"AmountMax={query.AmountMax.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            parts.Add($"Search={Uri.EscapeDataString(query.Search)}");
        }

        if (query.Currency.HasValue)
        {
            parts.Add($"Currency={query.Currency.Value}");
        }

        return $"{_basePath}?{string.Join("&", parts)}";
    }
}
