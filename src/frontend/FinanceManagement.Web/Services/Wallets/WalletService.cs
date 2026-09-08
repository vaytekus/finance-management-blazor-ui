using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Web.Common;
using FinanceManagement.Web.Services.Common;

namespace FinanceManagement.Web.Services.Wallets;

public class WalletService(HttpClient http) : IWalletService
{
    private const string _basePath = "api/wallets";

    public async Task<IReadOnlyList<WalletResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<PagedResult<WalletResponse>>($"{_basePath}?PageSize=100", ApiJsonOptions.Default, ct);
        return result?.Items ?? [];
    }

    public async Task<WalletResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{_basePath}/{id}", ct);
        return await response.ReadOrNullAsync<WalletResponse>(ct);
    }

    public async Task<WalletResponse> CreateAsync(CreateWalletRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(_basePath, request, ApiJsonOptions.Default, ct);
        return await response.ReadRequiredAsync<WalletResponse>(ct);
    }

    public async Task UpdateAsync(Guid id, UpdateWalletRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{_basePath}/{id}", request, ApiJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"{_basePath}/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
