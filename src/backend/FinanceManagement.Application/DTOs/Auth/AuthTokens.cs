namespace FinanceManagement.Application.DTOs.Auth;

public class AuthTokens
{
    public required string AccessToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
