using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Services;

public interface ITokenIssuer
{
    AuthTokens Issue(User user);
}
