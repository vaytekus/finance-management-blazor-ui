using FinanceManagement.Contracts.Users;

namespace FinanceManagement.Application.DTOs.Auth;

public class AuthResult
{
    public required AuthTokens Tokens { get; set; }
    public required UserResponse User { get; set; }
}
