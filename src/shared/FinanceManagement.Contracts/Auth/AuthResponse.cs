using FinanceManagement.Contracts.Users;

namespace FinanceManagement.Contracts.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = null!;
}
