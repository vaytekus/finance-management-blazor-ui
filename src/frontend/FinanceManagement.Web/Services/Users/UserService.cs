using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.Users;
using FinanceManagement.Web.Common;

namespace FinanceManagement.Web.Services.Users;

public class UserService(HttpClient http) : IUserService
{
    private const string _basePath = "api/users";

    public async Task<PagedResult<UserResponse>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null, CancellationToken ct = default)
    {
        var url = $"{_basePath}?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        var response = await http.GetAsync(url, ct);
        return await response.ReadRequiredAsync<PagedResult<UserResponse>>(ct);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{_basePath}/{id}", ct);
        return await response.ReadOrNullAsync<UserResponse>(ct);
    }

    public async Task<UserResponse> CreateAsync(AdminCreateUserRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(_basePath, request, ct);
        return await response.ReadRequiredAsync<UserResponse>(ct);
    }

    public async Task UpdateAsync(Guid id, UpdateUserProfileRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{_basePath}/{id}", request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);
    }

    public async Task ChangeRoleAsync(Guid id, ChangeUserRoleRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{_basePath}/{id}/role", request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);
    }

    public async Task ResetPasswordAsync(Guid id, AdminResetPasswordRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{_basePath}/{id}/password", request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"{_basePath}/{id}", ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);
    }
}
