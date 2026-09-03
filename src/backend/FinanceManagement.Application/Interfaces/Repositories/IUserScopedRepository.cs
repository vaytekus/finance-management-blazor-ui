namespace FinanceManagement.Application.Interfaces.Repositories;

public interface IUserScopedRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllForUserAsync(Guid userId, CancellationToken ct = default);
    Task<T?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default);

    void Add(T entity);
    void Update(T entity);
}
