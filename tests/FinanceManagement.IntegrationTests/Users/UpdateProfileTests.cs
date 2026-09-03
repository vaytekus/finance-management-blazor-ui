using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Users;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.IntegrationTests.Users;

public class UpdateProfileTests : IntegrationTestBase
{
    public UpdateProfileTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task UpdateMe_UpdatesUserNameAndEmail_InDb()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User, userName: "art");
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var request = new UpdateUserProfileRequest
        {
            UserName = "art_renamed",
            Email = "art_renamed@example.com"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/users/me", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var inDb = await ExecuteDbAsync(db =>
            db.Users.SingleAsync(u => u.Id == user.Id));
        inDb.UserName.Should().Be("art_renamed");
        inDb.Email.Should().Be("art_renamed@example.com");
    }
}
