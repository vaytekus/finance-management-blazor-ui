using FinanceManagement.Contracts.Users;

namespace FinanceManagement.Web.Services.Auth;

public interface IProfileService
{
    Task<UserResponse> GetMeAsync(CancellationToken ct = default);
    Task UpdateProfileAsync(UpdateUserProfileRequest request, CancellationToken ct = default);
    Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default);
}
