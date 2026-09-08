using System.Net.Http.Json;
using FinanceManagement.Contracts.Auth;
using FinanceManagement.Web.Common;

namespace FinanceManagement.Web.Services.Auth;

public class AuthService(HttpClient http, AuthStateService authState) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(ct)
            ?? throw new InvalidOperationException("Empty login response");

        authState.SetSession(body.AccessToken, body.ExpiresAt, body.User);

        return body;
    }
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/auth/register", request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(ct)
            ?? throw new InvalidOperationException("Empty register response");

        authState.SetSession(body.AccessToken, body.ExpiresAt, body.User);

        return body;
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        try
        {
            await http.PostAsync("api/auth/logout", content: null, ct);
        }
        finally
        {
            authState.Clear();
        }
    }

    public async Task<bool> TryRestoreSessionAsync(CancellationToken ct = default)
    {
        var response = await http.PostAsync("api/auth/refresh", content: null, ct);

        if (!response.IsSuccessStatusCode)
        {
            authState.Clear();
            return false;
        }

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(ct);
        if (body is null || string.IsNullOrEmpty(body.AccessToken) || body.User is null)
        {
            authState.Clear();
            return false;
        }

        authState.SetSession(body.AccessToken, body.ExpiresAt, body.User);
        return true;
    }
}
