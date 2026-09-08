using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Auth;
using FinanceManagement.Web.Services.Auth;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace FinanceManagement.Web.Services.Http;

public class AuthDelegatingHandler(AuthStateService authState) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken ct)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        if (authState.IsAuthenticated)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authState.AccessToken);
        }

        var response = await base.SendAsync(request, ct);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        if (IsAuthEndpoint(request))
        {
            return response;
        }

        var refreshed = await TryRefreshAsync(ct);

        if (!refreshed)
        {
            return response;
        }

        response.Dispose();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authState.AccessToken);

        return await base.SendAsync(request, ct);
    }

    private async Task<bool> TryRefreshAsync(CancellationToken ct)
    {
        using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
        refreshRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        using var refreshResponse = await base.SendAsync(refreshRequest, ct);
        if (!refreshResponse.IsSuccessStatusCode)
        {
            authState.Clear();
            return false;
        }

        var body = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>(ct);
        if (body is null || string.IsNullOrEmpty(body.AccessToken) || body.User is null)
        {
            authState.Clear();
            return false;
        }

        authState.SetSession(body.AccessToken, body.ExpiresAt, body.User);
        return true;
    }

    private static bool IsAuthEndpoint(HttpRequestMessage request)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;

        return path.Contains("api/auth/", StringComparison.OrdinalIgnoreCase);
    }
}
