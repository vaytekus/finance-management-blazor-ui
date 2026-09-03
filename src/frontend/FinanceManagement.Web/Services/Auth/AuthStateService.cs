using FinanceManagement.Contracts.Users;

namespace FinanceManagement.Web.Services.Auth;

public class AuthStateService
{
    public string? AccessToken { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public UserResponse? User { get; private set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(AccessToken)
        && ExpiresAt.HasValue
        && ExpiresAt.Value > DateTime.UtcNow;

    public event Action? OnChange;

    public void SetSession(string accessToken, DateTime expiresAt, UserResponse user)
    {
        AccessToken = accessToken;
        ExpiresAt = expiresAt;
        User = user;
        NotifyChanged();
    }

    public void UpdateUser(UserResponse user)
    {
        User = user;
        NotifyChanged();
    }

    public void Clear()
    {
        AccessToken = null;
        ExpiresAt = null;
        User = null;
        NotifyChanged();
    }

    private void NotifyChanged() => OnChange?.Invoke();
}
