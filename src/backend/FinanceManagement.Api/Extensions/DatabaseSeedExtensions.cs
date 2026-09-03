using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Api.Extensions;

public static class DatabaseSeedExtensions
{
    private const string _adminUserName = "admin";
    private const string _adminEmail = "admin@finance.local";
    private const string _defaultAdminPassword = "admin";
    private const string _adminPasswordConfigKey = "SeedAdmin:Password";

    public static async Task SeedAdminAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;

        var userRepository = sp.GetRequiredService<IUserRepository>();
        var userProvisioning = sp.GetRequiredService<IUserProvisioningService>();
        var passwordHasher = sp.GetRequiredService<IPasswordHasher>();
        var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
        var configuration = sp.GetRequiredService<IConfiguration>();

        if (await userRepository.UserNameExistsAsync(_adminUserName))
        {
            return;
        }

        var password = configuration[_adminPasswordConfigKey] ?? _defaultAdminPassword;

        var admin = new User
        {
            UserName = _adminUserName,
            Email = _adminEmail,
            PasswordHash = passwordHasher.Hash(password),
            RoleId = UserRole.Admin,
            CreatedAt = DateTime.UtcNow,
        };

        userRepository.Add(admin);
        userProvisioning.AddDefaultsFor(admin);

        await unitOfWork.SaveChangesAsync();
    }
}
