using FinanceManagement.Contracts.Auth;

namespace FinanceManagement.Web.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
    Task<bool> TryRestoreSessionAsync(CancellationToken ct = default);
}
