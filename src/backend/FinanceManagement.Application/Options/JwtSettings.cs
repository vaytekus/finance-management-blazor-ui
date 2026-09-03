namespace FinanceManagement.Application.Options;

public class JwtSettings
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Key { get; set; }
    
    public int AccessTokenExpiresInMinutes { get; set; } = 15;
    public int RefreshTokenExpiresInDays { get; set; } = 7;
}
