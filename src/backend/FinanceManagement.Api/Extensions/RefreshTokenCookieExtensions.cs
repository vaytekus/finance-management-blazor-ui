namespace FinanceManagement.Api.Extensions;

public static class RefreshTokenCookieExtensions
{
    private const string _cookieName = "refreshToken";
    private const string _cookiePath = "/api/auth";

    public static void SetRefreshTokenCookie(this HttpResponse response, string token, DateTime expiresAt, bool secure)
    {
        response.Cookies.Append(_cookieName, token, BuildOptions(expiresAt, secure));
    }

    public static string? GetRefreshTokenCookie(this HttpRequest request)
    {
        return request.Cookies[_cookieName];
    }

    public static void DeleteRefreshTokenCookie(this HttpResponse response, bool secure)
    {
        response.Cookies.Delete(_cookieName,  BuildOptions(DateTime.UnixEpoch, secure));
    }

    private static CookieOptions BuildOptions(DateTime expiresAt, bool secure) => new()
    {
        HttpOnly = true,
        Secure = secure,
        SameSite = SameSiteMode.None, // SameSiteMode.Lax when same domain
        Path = _cookiePath,
        Expires = expiresAt,
    };
}
