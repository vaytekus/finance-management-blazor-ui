using FinanceManagement.Application.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace FinanceManagement.Infrastructure.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) {
        return BC.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        return BC.Verify(password, hash);
    }
}