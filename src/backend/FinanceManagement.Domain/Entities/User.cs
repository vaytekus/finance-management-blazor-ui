using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole RoleId { get; set; }
    public Role? Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
