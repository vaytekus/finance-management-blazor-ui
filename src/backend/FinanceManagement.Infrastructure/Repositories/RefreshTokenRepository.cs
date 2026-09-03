using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await _db.RefreshTokens
            .Include(x => x.User)
            .ThenInclude(u => u!.Role)
            .FirstOrDefaultAsync(r => r.Token == token, ct);
    }
    
    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        
        return await _db.RefreshTokens
            .Include(x => x.User)
            .ThenInclude(u => u!.Role)
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > now)
            .ToListAsync(ct);
    }
    public async Task<int> DeleteOlderThanAsync(DateTime cutoff, CancellationToken ct = default)
    {
        return await _db.RefreshTokens
            .Where(r => r.RevokedAt < cutoff || r.ExpiresAt < cutoff)
            .ExecuteDeleteAsync(ct);
    }

    public void Add(RefreshToken token)
    {
        _db.RefreshTokens.Add(token);
    }
    
    public void RevokeAll(IEnumerable<RefreshToken> tokens, string reason)
    {
        var now = DateTime.UtcNow;

        foreach (var token in tokens)
        {
            if (token.RevokedAt is null)
            {
                token.RevokedAt = now;
                token.ReasonRevoked = reason;
            }
        }
    }
}
