namespace FinanceManagement.Contracts.Users;

public class UpdateUserProfileRequest
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
