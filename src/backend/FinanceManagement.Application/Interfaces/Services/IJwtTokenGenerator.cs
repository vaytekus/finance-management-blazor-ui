using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Services;

public interface IJwtTokenGenerator
{
    (string AccessToken, DateTime ExpiresAt) GenerateToken(User user);
}
