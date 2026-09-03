using System.Security.Cryptography;
using FinanceManagement.Application.Interfaces.Services;

namespace FinanceManagement.Infrastructure.Services;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int _tokenSizeBytes = 64;

    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(_tokenSizeBytes);
        return Convert.ToBase64String(bytes);
    }
}
