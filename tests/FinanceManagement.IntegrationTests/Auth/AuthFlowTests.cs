using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Auth;
using FinanceManagement.Contracts.Users;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.IntegrationTests.Auth;

public class AuthFlowTests : IntegrationTestBase
{
    public AuthFlowTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Register_WithValidData_Returns200_AndCreatesUserInDb()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "alice",
            Email = "alice@test.local",
            Password = "P@ssw0rd"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.AccessToken.Should().NotBeNullOrEmpty();
        auth.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        var userInDb = await ExecuteDbAsync(db =>
            db.Users.SingleOrDefaultAsync(u => u.UserName == "alice"));

        userInDb.Should().NotBeNull();
        userInDb.Email.Should().Be("alice@test.local");
        userInDb.RoleId.Should().Be(UserRole.User);
        userInDb.PasswordHash.Should().NotBe("P@ssw0rd");
    }

    [Fact]
    public async Task Login_AfterUserExists_Returns200_WithAccessToken()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User, userName: "art");

        var request = new LoginRequest
        {
            UserName = user.UserName,
            Password = DefaultPassword
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetMe_WithValidToken_ReturnsCurrentUser()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User, userName: "art");
        var authedClient = await CreateAuthenticatedClientAsync(user.UserName);

        // Act
        var response = await authedClient.GetAsync("/api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var me = await response.Content.ReadFromJsonAsync<UserResponse>();
        me!.Id.Should().Be(user.Id);
        me.UserName.Should().Be("art");
        me.Role.Should().Be(nameof(UserRole.User));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401Unauthorized()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User, userName: "eve");
        var request = new LoginRequest
        {
            UserName = user.UserName,
            Password = "wrong-password"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithDuplicateUserName_Returns409Conflict()
    {
        // Arrange
        await CreateUserAsync(UserRole.User, userName: "duplicate");

        var request = new RegisterRequest
        {
            UserName = "duplicate",
            Email = "another@test.local",
            Password = "P@ssw0rd"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}