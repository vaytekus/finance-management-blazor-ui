using FinanceManagement.Domain.Common;
using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Domain.Entities;

public class Wallet : ISoftDeletable
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Currency Currency { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
