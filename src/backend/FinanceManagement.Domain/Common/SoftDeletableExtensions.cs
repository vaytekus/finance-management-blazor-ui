namespace FinanceManagement.Domain.Common;

public static class SoftDeletableExtensions
{
    public static void MarkDeleted(this ISoftDeletable entity)
    {
        entity.DeletedAt = DateTime.UtcNow;
    }
}
