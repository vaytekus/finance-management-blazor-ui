using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Auth;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.IntegrationTests.Auth;

public class RefreshTokenTests : IntegrationTestBase
{
    public RefreshTokenTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Refresh_WithValidToken_RotatesAndReturnsNewTokens()
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

        var oldRefreshToken = ExtractRefreshCookie(loginResp);

        // Act
        var refreshResp = await client.PostAsync("/api/auth/refresh", content: null);

        // Assert
        refreshResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshAuth = await refreshResp.Content.ReadFromJsonAsync<AuthResponse>();
        refreshAuth!.AccessToken.Should().NotBeNullOrEmpty();
        refreshAuth.AccessToken.Should().NotBe(loginAuth!.AccessToken);

        var newRefreshToken = ExtractRefreshCookie(refreshResp);
        newRefreshToken.Should().NotBe(oldRefreshToken);

        var oldInDb = await ExecuteDbAsync(db =>
            db.RefreshTokens.SingleOrDefaultAsync(t => t.Token == oldRefreshToken));
        oldInDb.Should().NotBeNull();
        oldInDb.RevokedAt.Should().NotBeNull();
        oldInDb.ReasonRevoked.Should().Be("Rotated");
        oldInDb.ReplacedByToken.Should().Be(newRefreshToken);
    }

    [Fact]
    public async Task Refresh_WithReusedToken_RevokesAllUserTokens()
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
        var oldRefreshToken = ExtractRefreshCookie(loginResp);

        var firstRefreshResp = await client.PostAsync("/api/auth/refresh", content: null);
        firstRefreshResp.EnsureSuccessStatusCode();
        var newRefreshToken = ExtractRefreshCookie(firstRefreshResp);

        // Act 
        var reuseRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh");
        reuseRequest.Headers.Add("Cookie", $"refreshToken={Uri.EscapeDataString(oldRefreshToken)}");

        var attackerClient = Factory.CreateClient();
        var reuseResp = await attackerClient.SendAsync(reuseRequest);

        // Assert
        reuseResp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        var tokens = await ExecuteDbAsync(db =>
            db.RefreshTokens.Where(t => t.UserId == user.Id).ToListAsync());

        tokens.Should().HaveCount(2);
        tokens.Should().OnlyContain(t => t.RevokedAt != null);

        var oldInDb = tokens.Single(t => t.Token == oldRefreshToken);
        oldInDb.ReasonRevoked.Should().Be("Rotated");

        var newInDb = tokens.Single(t => t.Token == newRefreshToken);
        newInDb.ReasonRevoked.Should().Be("Reuse detected");
    }

    [Fact]
    public async Task Logout_RevokesRefreshToken_AndDeletesCookie()
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
        var refreshToken = ExtractRefreshCookie(loginResp);

        // Act
        var logoutResp = await client.PostAsync("/api/auth/logout", content: null);

        // Assert
        logoutResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var setCookie = logoutResp.Headers.GetValues("Set-Cookie")
            .First(c => c.StartsWith("refreshToken="));
        setCookie.Should().Contain("expires=");

        var tokenInDb = await ExecuteDbAsync(db =>
            db.RefreshTokens.SingleOrDefaultAsync(t => t.Token == refreshToken));
        tokenInDb.Should().NotBeNull();
        tokenInDb!.RevokedAt.Should().NotBeNull();
        tokenInDb.ReasonRevoked.Should().Be("Logout");

        var refreshAfterLogoutResp = await client.PostAsync("/api/auth/refresh", content: null);
        refreshAfterLogoutResp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static string ExtractRefreshCookie(HttpResponseMessage response)
    {
        var setCookie = response.Headers.GetValues("Set-Cookie")
            .First(c => c.StartsWith("refreshToken="));
        var raw = setCookie.Split(';')[0]["refreshToken=".Length..];
        return Uri.UnescapeDataString(raw);
    }
}