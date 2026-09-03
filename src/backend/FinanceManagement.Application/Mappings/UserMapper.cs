using FinanceManagement.Contracts.Users;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Mappings;

public static class UserMapper
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Role = user.Role?.Name ?? user.RoleId.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
