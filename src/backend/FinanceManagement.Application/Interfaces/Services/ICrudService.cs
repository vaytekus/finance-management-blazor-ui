namespace FinanceManagement.Application.Interfaces.Services;

public interface ICrudService<TResponse, TCreate, TUpdate>
{
    Task<IReadOnlyList<TResponse>> GetAllAsync(CancellationToken ct = default);
    Task<TResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TResponse> CreateAsync(TCreate request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, TUpdate request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
