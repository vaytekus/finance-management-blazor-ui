using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Domain.Entities;

public class Role
{
    public UserRole Id { get; set; }
    public required string Name { get; set; }
}
