using System.Net.Http.Json;
using FinanceManagement.Contracts.Auth;
using FinanceManagement.Web.Common;

namespace FinanceManagement.Web.Services.Auth;

public class AuthService: IAuthService
{
    private readonly HttpClient _http;
    private readonly AuthStateService _authState;

    public AuthService(HttpClient http, AuthStateService authState)
    {
        _http = http;
        _authState = authState;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(ct)
            ?? throw new InvalidOperationException("Empty login response");

        _authState.SetSession(body.AccessToken, body.ExpiresAt, body.User);

        return body;
    }
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request, ct);
        await response.EnsureSuccessOrThrowApiErrorAsync(ct);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(ct)
            ?? throw new InvalidOperationException("Empty register response");

        _authState.SetSession(body.AccessToken, body.ExpiresAt, body.User);

        return body;
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        try
        {
            await _http.PostAsync("api/auth/logout", content: null, ct);
        }
        finally
        {
            _authState.Clear();
        }
    }

    public async Task<bool> TryRestoreSessionAsync(CancellationToken ct = default)
    {
        var response = await _http.PostAsync("api/auth/refresh", content: null, ct);

        if (!response.IsSuccessStatusCode)
        {
            _authState.Clear();
            return false;
        }

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(ct);
        if (body is null || string.IsNullOrEmpty(body.AccessToken) || body.User is null)
        {
            _authState.Clear();
            return false;
        }

        _authState.SetSession(body.AccessToken, body.ExpiresAt, body.User);
        return true;
    }
}
