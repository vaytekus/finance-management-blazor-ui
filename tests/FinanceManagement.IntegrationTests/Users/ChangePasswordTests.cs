using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Auth;
using FinanceManagement.Contracts.Users;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.IntegrationTests.Users;

public class ChangePasswordTests : IntegrationTestBase
{
    public ChangePasswordTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task ChangePassword_WithValidCurrentPassword_UpdatesHash_AndRevokesActiveTokens()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User, userName: "art");
        var client = Factory.CreateClient();

        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            UserName = user.UserName,
            Password = DefaultPassword
        });
        loginResp.EnsureSuccessStatusCode();
        var loginAuth = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginAuth!.AccessToken);

        var oldHash = await ExecuteDbAsync(db =>
            db.Users.Where(u => u.Id == user.Id).Select(u => u.PasswordHash).SingleAsync());

        var request = new ChangePasswordRequest
        {
            CurrentPassword = DefaultPassword,
            NewPassword = "N3wStr0ng!"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/users/me/password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var newHash = await ExecuteDbAsync(db =>
            db.Users.Where(u => u.Id == user.Id).Select(u => u.PasswordHash).SingleAsync());
        newHash.Should().NotBe(oldHash);
        newHash.Should().NotBe("N3wStr0ng!");

        var tokens = await ExecuteDbAsync(db =>
            db.RefreshTokens.Where(t => t.UserId == user.Id).ToListAsync());
        tokens.Should().OnlyContain(t => t.RevokedAt != null);
        tokens.Should().OnlyContain(t => t.ReasonRevoked == "Password changed");

        var loginWithNewResp = await Factory.CreateClient().PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            UserName = user.UserName,
            Password = "N3wStr0ng!"
        });
        loginWithNewResp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_Returns401_AndDoesNotUpdate()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User, userName: "art");
        var client = Factory.CreateClient();

        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            UserName = user.UserName,
            Password = DefaultPassword
        });
        loginResp.EnsureSuccessStatusCode();
        var loginAuth = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginAuth!.AccessToken);

        var originalHash = await ExecuteDbAsync(db =>
            db.Users.Where(u => u.Id == user.Id).Select(u => u.PasswordHash).SingleAsync());

        var request = new ChangePasswordRequest
        {
            CurrentPassword = "wrong-password",
            NewPassword = "N3wStr0ng!"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/users/me/password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var currentHash = await ExecuteDbAsync(db =>
            db.Users.Where(u => u.Id == user.Id).Select(u => u.PasswordHash).SingleAsync());
        currentHash.Should().Be(originalHash);

        var tokens = await ExecuteDbAsync(db =>
            db.RefreshTokens.Where(t => t.UserId == user.Id).ToListAsync());
        tokens.Should().OnlyContain(t => t.RevokedAt == null);
    }
}