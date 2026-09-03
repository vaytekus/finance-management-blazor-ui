using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.Users;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.IntegrationTests.Users;

public class UsersAdminTests : IntegrationTestBase
{
    public UsersAdminTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAllUsers_AsRegularUser_Returns403Forbidden()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllUsers_AsAdmin_ReturnsPagedList()
    {
        // Arrange
        var admin = await CreateUserAsync(UserRole.Admin, userName: "admin1");
        await CreateUserAsync(UserRole.User, userName: "reg1");
        await CreateUserAsync(UserRole.User, userName: "reg2");

        var adminClient = await CreateAuthenticatedClientAsync(admin.UserName);

        // Act
        var response = await adminClient.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var page = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>(JsonOptions);

        page!.TotalCount.Should().Be(3);
        page.Items.Should().Contain(u => u.UserName == "admin1");
        page.Items.Should().Contain(u => u.UserName == "reg1");
        page.Items.Should().Contain(u => u.UserName == "reg2");
    }

    [Fact]
    public async Task GetUserById_AsAdmin_ReturnsUser()
    {
        // Arrange
        var admin = await CreateUserAsync(UserRole.Admin, userName: "admin1");
        var target = await CreateUserAsync(UserRole.User, userName: "target");

        var adminClient = await CreateAuthenticatedClientAsync(admin.UserName);

        // Act
        var response = await adminClient.GetAsync($"/api/users/{target.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        body!.Id.Should().Be(target.Id);
        body.UserName.Should().Be("target");
    }

    [Fact]
    public async Task DeleteUser_AsAdmin_RemovesFromDb()
    {
        // Arrange
        var admin = await CreateUserAsync(UserRole.Admin, userName: "admin1");
        var target = await CreateUserAsync(UserRole.User, userName: "victim");

        var adminClient = await CreateAuthenticatedClientAsync(admin.UserName);

        // Act
        var response = await adminClient.DeleteAsync($"/api/users/{target.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var inDb = await ExecuteDbAsync(db =>
            db.Users.SingleOrDefaultAsync(u => u.Id == target.Id));
        inDb.Should().BeNull();
    }

    [Fact]
    public async Task ChangeUserRole_AsAdmin_UpdatesRoleInDb()
    {
        // Arrange
        var admin = await CreateUserAsync(UserRole.Admin, userName: "admin1");
        var target = await CreateUserAsync(UserRole.User, userName: "victim");

        var adminClient = await CreateAuthenticatedClientAsync(admin.UserName);
        var request = new ChangeUserRoleRequest { Role = nameof(UserRole.Admin) };

        // Act
        var response = await adminClient.PutAsJsonAsync($"/api/users/{target.Id}/role", request);

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert — DB
        var inDb = await ExecuteDbAsync(db =>
            db.Users.SingleAsync(u => u.Id == target.Id));

        inDb.RoleId.Should().Be(UserRole.Admin);
    }
}