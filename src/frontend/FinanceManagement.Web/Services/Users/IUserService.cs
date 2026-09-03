using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.Users;

namespace FinanceManagement.Web.Services.Users;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null, CancellationToken ct = default);
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserResponse> CreateAsync(AdminCreateUserRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateUserProfileRequest request, CancellationToken ct = default);
    Task ChangeRoleAsync(Guid id, ChangeUserRoleRequest request, CancellationToken ct = default);
    Task ResetPasswordAsync(Guid id, AdminResetPasswordRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
