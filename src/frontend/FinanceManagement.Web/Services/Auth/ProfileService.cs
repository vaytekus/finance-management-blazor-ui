using System.Net.Http.Json;
using FinanceManagement.Contracts.Users;
using FinanceManagement.Web.Common;

namespace FinanceManagement.Web.Services.Auth;

public class ProfileService(HttpClient http) : IProfileService
{
    private const string _basePath = "api/users/me";

    public async Task<UserResponse> GetMeAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync(_basePath, ct);
        return await response.ReadRequiredAsync<UserResponse>(ct);
    }

    public async Task UpdateProfileAsync(UpdateUserProfileRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync(_basePath, request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{_basePath}/password", request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);
    }
}
