using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.Features.Users.Queries.GetAllUsers;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using FinanceManagement.Infrastructure.Data;
using FinanceManagement.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(x => x.UserName == userName, ct);
    }
    
    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .OrderBy(u => u.CreatedAt)
            .ToListAsync(ct);
    }
    
    public async Task<bool> UserNameExistsAsync(string userName, CancellationToken ct = default)
    {
        return await _db.Users
            .AnyAsync(x => x.UserName == userName, ct);
    }
    
    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await _db.Users
            .AnyAsync(x => x.Email == email, ct);
    }
    
    public async Task<bool> AnyOtherAdminAsync(Guid excludingUserId, CancellationToken ct = default)
    {
        return await _db.Users
            .AnyAsync(u => u.Id != excludingUserId && u.RoleId == UserRole.Admin, ct);
    }
    
    public async Task DeleteOperationsAsync(Guid userId, CancellationToken ct = default)
    {
        await _db.Operations
            .IgnoreQueryFilters()
            .Where(o => o.Wallet!.UserId == userId)
            .ExecuteDeleteAsync(ct);
    }

    public void Add(User user)
    {
        _db.Users.Add(user);
    }
    
    public void Remove(User user)
    {
        _db.Users.Remove(user);
    }
    public async Task<PagedResult<User>> GetPagedAsync(GetAllUsersQuery query, CancellationToken ct = default)
    {
        var q = _db.Users
            .Include(u => u.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            q = q.Where(u => u.Role!.Name == query.Role);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search}%";
            q = q.Where(u =>
                EF.Functions.ILike(u.UserName, pattern) ||
                EF.Functions.ILike(u.Email, pattern));
        }

        q = (query.SortBy?.ToLowerInvariant(), query.SortOrder) switch
        {
            ("username", SortOrder.Asc) => q.OrderBy(u => u.UserName),
            ("username", SortOrder.Desc) => q.OrderByDescending(u => u.UserName),

            ("email", SortOrder.Asc) => q.OrderBy(u => u.Email),
            ("email", SortOrder.Desc) => q.OrderByDescending(u => u.Email),

            ("role", SortOrder.Asc) => q.OrderBy(u => u.Role!.Name),
            ("role", SortOrder.Desc) => q.OrderByDescending(u => u.Role!.Name),
            ("createdat", SortOrder.Desc) => q.OrderByDescending(u => u.CreatedAt),
            (_, SortOrder.Desc) => q.OrderByDescending(u => u.CreatedAt),
            _ => q.OrderBy(u => u.CreatedAt),
        };

        return await q.ToPagedResultAsync(query.Page, query.PageSize, ct);
    }
}
