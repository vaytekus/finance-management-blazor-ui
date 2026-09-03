using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Features.Users.Queries.GetAllUsers;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<bool> UserNameExistsAsync(string userName, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> AnyOtherAdminAsync(Guid excludingUserId, CancellationToken ct = default);
    Task DeleteOperationsAsync(Guid userId, CancellationToken ct = default);
    
    
    void Add(User user);
    void Remove(User user);
    
    Task<PagedResult<User>> GetPagedAsync(
        GetAllUsersQuery query,
        CancellationToken ct = default);
}
