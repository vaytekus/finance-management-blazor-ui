using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace FinanceManagement.Web.Services.Auth;

public class AppAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthStateService _authState;
    private static readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public AppAuthenticationStateProvider(AuthStateService authState)
    {
        _authState = authState;
        _authState.OnChange += HandleAuthStateChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_authState.IsAuthenticated || _authState.AccessToken is null)
        {
            return Task.FromResult(_anonymous);
        }

        var claims = JwtClaimsParser.Parse(_authState.AccessToken);
        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    private void HandleAuthStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public void Dispose()
    {
        _authState.OnChange -= HandleAuthStateChanged;
    }
}
