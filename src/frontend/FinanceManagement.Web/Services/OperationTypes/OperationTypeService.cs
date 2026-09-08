using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Web.Common;
using FinanceManagement.Web.Services.Common;

namespace FinanceManagement.Web.Services.OperationTypes;

public class OperationTypeService(HttpClient http) : IOperationTypeService
{
    private const string _basePath = "api/operation-types";

    public async Task<IReadOnlyList<OperationTypeResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<PagedResult<OperationTypeResponse>>($"{_basePath}?PageSize=100", ApiJsonOptions.Default, ct);
        return result?.Items ?? [];
    }

    public async Task<OperationTypeResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{_basePath}/{id}", ct);
        return await response.ReadOrNullAsync<OperationTypeResponse>(ct);
    }

    public async Task<OperationTypeResponse> CreateAsync(CreateOperationTypeRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync($"{_basePath}", request, ApiJsonOptions.Default, ct);
        return await response.ReadRequiredAsync<OperationTypeResponse>(ct);
    }

    public async Task UpdateAsync(Guid id, UpdateOperationTypeRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{_basePath}/{id}", request, ApiJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<int> CountOperationsAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{_basePath}/{id}/operations-count", ct);
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

        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }
}
