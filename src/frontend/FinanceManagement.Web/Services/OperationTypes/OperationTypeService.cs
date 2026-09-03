using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Web.Common;
using FinanceManagement.Web.Services.Common;

namespace FinanceManagement.Web.Services.OperationTypes;

public class OperationTypeService : IOperationTypeService
{
    private const string _basePath = "api/operation-types";

    private readonly HttpClient _http;

    public OperationTypeService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<OperationTypeResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<PagedResult<OperationTypeResponse>>($"{_basePath}?PageSize=100", ApiJsonOptions.Default, ct);
        return result?.Items ?? [];
    }

    public async Task<OperationTypeResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"{_basePath}/{id}", ct);
        return await response.ReadOrNullAsync<OperationTypeResponse>(ct);
    }

    public async Task<OperationTypeResponse> CreateAsync(CreateOperationTypeRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync($"{_basePath}", request, ApiJsonOptions.Default, ct);
        return await response.ReadRequiredAsync<OperationTypeResponse>(ct);
    }

    public async Task UpdateAsync(Guid id, UpdateOperationTypeRequest request, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"{_basePath}/{id}", request, ApiJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<int> CountOperationsAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"{_basePath}/{id}/operations-count", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task DeleteAsync(Guid id, DeleteOperationTypeRequest? body = null, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_basePath}/{id}");
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: ApiJsonOptions.Default);
        }

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }
}
