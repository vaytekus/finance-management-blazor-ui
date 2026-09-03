using System.Security.Claims;
using System.Text.Json;

namespace FinanceManagement.Web.Services.Auth;

public static class JwtClaimsParser
{
    public static IEnumerable<Claim> Parse(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var pairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        return pairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "==";
                break;

            case 3: base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}
