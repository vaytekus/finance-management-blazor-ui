using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.OperationTypes;
using ContractsOperationKind = FinanceManagement.Contracts.Enums.OperationKind;
using FinanceManagement.Application.Features.OperationTypes.Commands.CreateOperationType;
using FinanceManagement.Application.Features.OperationTypes.Commands.UpdateOperationType;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.IntegrationTests.OperationTypes;

public class OperationTypesTests : IntegrationTestBase
{
    public OperationTypesTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateOperationType_ReturnsCreated_AndPersistsInDb()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var command = new CreateOperationTypeCommand(
            Name: "Salary",
            Description: "Monthly income",
            Kind: OperationKind.Income);

        // Act
        var response = await client.PostAsJsonAsync("/api/operation-types", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<OperationTypeResponse>(JsonOptions);
        created!.Id.Should().NotBeEmpty();
        created.Name.Should().Be("Salary");
        created.Kind.Should().Be(ContractsOperationKind.Income);

        var inDb = await ExecuteDbAsync(db =>
            db.OperationTypes.SingleOrDefaultAsync(t => t.Id == created.Id));
        inDb.Should().NotBeNull();
        inDb.UserId.Should().Be(user.Id);
        inDb.Description.Should().Be("Monthly income");
    }

    [Fact]
    public async Task CreateOperationType_WithDuplicateName_Returns409()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var existing = new OperationType
        {
            Id = Guid.NewGuid(),
            Name = "Salary",
            Kind = OperationKind.Income,
            UserId = user.Id
        };
        await ExecuteDbAsync(async db =>
        {
            db.OperationTypes.Add(existing);
            await db.SaveChangesAsync();
        });

        var command = new CreateOperationTypeCommand(
            Name: "Salary",
            Description: "Dup attempt",
            Kind: OperationKind.Income);

        // Act
        var response = await client.PostAsJsonAsync("/api/operation-types", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var count = await ExecuteDbAsync(db =>
            db.OperationTypes.CountAsync(t => t.UserId == user.Id && t.Name == "Salary"));
        count.Should().Be(1);
    }

    [Fact]
    public async Task GetAllOperationTypes_ReturnsOnlyOwned()
    {
        // Arrange
        var art = await CreateUserAsync(UserRole.User, userName: "art");
        var max = await CreateUserAsync(UserRole.User, userName: "max");

        var artType = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Art Salary",
            Kind = OperationKind.Income, UserId = art.Id
        };
        var maxType = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Max Salary",
            Kind = OperationKind.Income, UserId = max.Id
        };
        await ExecuteDbAsync(async db =>
        {
            db.OperationTypes.AddRange(artType, maxType);
            await db.SaveChangesAsync();
        });

        var artClient = await CreateAuthenticatedClientAsync(art.UserName);

        // Act
        var response = await artClient.GetAsync("/api/operation-types");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var page = await response.Content.ReadFromJsonAsync<PagedResult<OperationTypeResponse>>(JsonOptions);

        page!.TotalCount.Should().Be(1);
        page.Items.Should().ContainSingle(t => t.Id == artType.Id);
        page.Items.Should().NotContain(t => t.Id == maxType.Id);
    }

    [Fact]
    public async Task GetOperationTypeById_ForForeignType_Returns404()
    {
        // Arrange
        var art = await CreateUserAsync(UserRole.User, userName: "art");
        var max = await CreateUserAsync(UserRole.User, userName: "max");

        var maxType = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Max Salary",
            Kind = OperationKind.Income, UserId = max.Id
        };
        await ExecuteDbAsync(async db =>
        {
            db.OperationTypes.Add(maxType);
            await db.SaveChangesAsync();
        });

        var artClient = await CreateAuthenticatedClientAsync(art.UserName);

        // Act
        var response = await artClient.GetAsync($"/api/operation-types/{maxType.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOperationType_UpdatesNameAndDescription_InDb()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var type = new OperationType
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Description = "old desc",
            Kind = OperationKind.Income,
            UserId = user.Id
        };
        await ExecuteDbAsync(async db =>
        {
            db.OperationTypes.Add(type);
            await db.SaveChangesAsync();
        });

        var command = new UpdateOperationTypeCommand(
            Id: Guid.Empty,
            Name: "New Name",
            Description: "new desc",
            Kind: OperationKind.Income);

        // Act
        var response = await client.PutAsJsonAsync($"/api/operation-types/{type.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var inDb = await ExecuteDbAsync(db =>
            db.OperationTypes.SingleAsync(t => t.Id == type.Id));
        inDb.Name.Should().Be("New Name");
        inDb.Description.Should().Be("new desc");
    }

    [Fact]
    public async Task DeleteOperationType_WhenUsedInOperations_Returns409()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Cash", Currency = Currency.UAH,
            UserId = user.Id, CreatedAt = DateTime.UtcNow
        };
        var type = new OperationType
        {
            Id = Guid.NewGuid(),
            Name = "Salary",
            Kind = OperationKind.Income,
            UserId = user.Id
        };
        var op = new Operation
        {
            Id = Guid.NewGuid(),
            TypeId = type.Id,
            WalletId = wallet.Id,
            Amount = 100m,
            Date = DateTime.UtcNow
        };

        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(wallet);
            db.OperationTypes.Add(type);
            db.Operations.Add(op);
            await db.SaveChangesAsync();
        });

        // Act
        var response = await client.DeleteAsync($"/api/operation-types/{type.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var inDb = await ExecuteDbAsync(db =>
            db.OperationTypes.SingleOrDefaultAsync(t => t.Id == type.Id));
        inDb.Should().NotBeNull("тип не має бути видалений якщо є операції що на нього посилаються");
    }
}