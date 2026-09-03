using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<int> DeleteOlderThanAsync(DateTime cutoff, CancellationToken ct = default);
    
    void Add(RefreshToken token);
    void RevokeAll(IEnumerable<RefreshToken> tokens, string reason);
}
