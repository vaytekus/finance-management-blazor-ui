using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.Operations;
using ContractsOperationKind = FinanceManagement.Contracts.Enums.OperationKind;
using FinanceManagement.Application.Features.Operations.Commands.CreateOperation;
using FinanceManagement.Application.Features.Operations.Commands.UpdateOperation;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.IntegrationTests.Operations;

public class OperationsTests : IntegrationTestBase
{
    public OperationsTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateOperation_ReturnsCreated_AndPersistsInDb()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var wallet = new Wallet
        {
            Name = "Main", Currency = Currency.USD,
            UserId = user.Id, CreatedAt = DateTime.UtcNow
        };
        var type = new OperationType
        {
            Id = Guid.NewGuid(),
            Name = "Salary",
            Kind = OperationKind.Income,
            UserId = user.Id
        };

        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(wallet);
            db.OperationTypes.Add(type);
            await db.SaveChangesAsync();
        });

        var command = new CreateOperationCommand(
            TypeId: type.Id,
            WalletId: wallet.Id,
            Amount: 1500m,
            Date: DateTime.UtcNow,
            Note: "October pay",
            Currency: Currency.USD);

        // Act
        var response = await client.PostAsJsonAsync("/api/operations", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<OperationResponse>(JsonOptions);
        created!.Id.Should().NotBeEmpty();
        created.Amount.Should().Be(1500m);
        created.WalletId.Should().Be(wallet.Id);
        created.TypeId.Should().Be(type.Id);
        created.TypeName.Should().Be("Salary");
        created.Kind.Should().Be(ContractsOperationKind.Income);

        var inDb = await ExecuteDbAsync(db =>
            db.Operations.SingleOrDefaultAsync(o => o.Id == created.Id));
        inDb.Should().NotBeNull();
        inDb.Amount.Should().Be(1500m);
        inDb.WalletId.Should().Be(wallet.Id);
        inDb.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task CreateOperation_WithForeignWallet_Returns404()
    {
        // Arrange
        var art = await CreateUserAsync(UserRole.User, userName: "art");
        var max = await CreateUserAsync(UserRole.User, userName: "max");

        var artType = new OperationType
        {
            Id = Guid.NewGuid(),
            Name = "Groceries",
            Kind = OperationKind.Expense,
            UserId = art.Id
        };
        var maxWallet = new Wallet
        {
            Name = "Max Wallet", Currency = Currency.USD,
            UserId = max.Id, CreatedAt = DateTime.UtcNow
        };

        await ExecuteDbAsync(async db =>
        {
            db.OperationTypes.Add(artType);
            db.Wallets.Add(maxWallet);
            await db.SaveChangesAsync();
        });

        var artClient = await CreateAuthenticatedClientAsync(art.UserName);

        var command = new CreateOperationCommand(
            TypeId: artType.Id,
            WalletId: maxWallet.Id,
            Amount: 100m,
            Date: DateTime.UtcNow,
            Note: null,
            Currency: Currency.USD);

        // Act
        var response = await artClient.PostAsJsonAsync("/api/operations", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var count = await ExecuteDbAsync(db => db.Operations.CountAsync());
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetOperationById_ForForeignOperation_Returns404()
    {
        // Arrange
        var art = await CreateUserAsync(UserRole.User, userName: "art");
        var max = await CreateUserAsync(UserRole.User, userName: "max");

        var maxWallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Max W", Currency = Currency.USD,
            UserId = max.Id, CreatedAt = DateTime.UtcNow
        };
        var maxType = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Max Type",
            Kind = OperationKind.Income, UserId = max.Id
        };
        var maxOp = new Operation
        {
            Id = Guid.NewGuid(), TypeId = maxType.Id, WalletId = maxWallet.Id,
            Amount = 500m, Date = DateTime.UtcNow
        };

        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(maxWallet);
            db.OperationTypes.Add(maxType);
            db.Operations.Add(maxOp);
            await db.SaveChangesAsync();
        });

        var artClient = await CreateAuthenticatedClientAsync(art.UserName);

        // Act
        var response = await artClient.GetAsync($"/api/operations/{maxOp.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOperation_UpdatesAmountAndNote_InDb()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Main", Currency = Currency.USD,
            UserId = user.Id, CreatedAt = DateTime.UtcNow
        };
        var type = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Salary",
            Kind = OperationKind.Income, UserId = user.Id
        };
        var op = new Operation
        {
            Id = Guid.NewGuid(), TypeId = type.Id, WalletId = wallet.Id,
            Amount = 100m, Date = DateTime.UtcNow, Note = "old"
        };
        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(wallet);
            db.OperationTypes.Add(type);
            db.Operations.Add(op);
            await db.SaveChangesAsync();
        });

        var command = new UpdateOperationCommand(
            Id: Guid.Empty,
            TypeId: type.Id,
            WalletId: wallet.Id,
            Amount: 999m,
            Date: op.Date,
            Note: "updated",
            Currency: Currency.USD);

        // Act
        var response = await client.PutAsJsonAsync($"/api/operations/{op.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var inDb = await ExecuteDbAsync(db =>
            db.Operations.SingleAsync(o => o.Id == op.Id));
        inDb.Amount.Should().Be(999m);
        inDb.Note.Should().Be("updated");
    }

    [Fact]
    public async Task DeleteOperation_SetsDeletedAt_AndHidesFromGetAll()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Main", Currency = Currency.USD,
            UserId = user.Id, CreatedAt = DateTime.UtcNow
        };
        var type = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Salary",
            Kind = OperationKind.Income, UserId = user.Id
        };
        var op = new Operation
        {
            Id = Guid.NewGuid(), TypeId = type.Id, WalletId = wallet.Id,
            Amount = 100m, Date = DateTime.UtcNow
        };
        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(wallet);
            db.OperationTypes.Add(type);
            db.Operations.Add(op);
            await db.SaveChangesAsync();
        });

        // Act
        var response = await client.DeleteAsync($"/api/operations/{op.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var inDb = await ExecuteDbAsync(db =>
            db.Operations.IgnoreQueryFilters()
                .SingleAsync(o => o.Id == op.Id));
        inDb.DeletedAt.Should().NotBeNull();
        inDb.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var getResponse = await client.GetAsync("/api/operations");
        var page = await getResponse.Content.ReadFromJsonAsync<PagedResult<OperationResponse>>(JsonOptions);
        page!.Items.Should().NotContain(o => o.Id == op.Id);
    }

    [Fact]
    public async Task GetMyOperations_ReturnsOnlyOwned()
    {
        // Arrange
        var art = await CreateUserAsync(UserRole.User, userName: "art");
        var max = await CreateUserAsync(UserRole.User, userName: "max");

        var artWallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Art W", Currency = Currency.USD,
            UserId = art.Id, CreatedAt = DateTime.UtcNow
        };
        var maxWallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Max W", Currency = Currency.USD,
            UserId = max.Id, CreatedAt = DateTime.UtcNow
        };
        var artType = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Art Type",
            Kind = OperationKind.Income, UserId = art.Id
        };
        var maxType = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Max Type",
            Kind = OperationKind.Income, UserId = max.Id
        };

        var artOp = new Operation
        {
            Id = Guid.NewGuid(), TypeId = artType.Id, WalletId = artWallet.Id,
            Amount = 100m, Date = DateTime.UtcNow
        };
        var maxOp = new Operation
        {
            Id = Guid.NewGuid(), TypeId = maxType.Id, WalletId = maxWallet.Id,
            Amount = 200m, Date = DateTime.UtcNow
        };

        await ExecuteDbAsync(async db =>
        {
            db.Wallets.AddRange(artWallet, maxWallet);
            db.OperationTypes.AddRange(artType, maxType);
            db.Operations.AddRange(artOp, maxOp);
            await db.SaveChangesAsync();
        });

        var artClient = await CreateAuthenticatedClientAsync(art.UserName);

        // Act
        var response = await artClient.GetAsync("/api/operations");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var page = await response.Content.ReadFromJsonAsync<PagedResult<OperationResponse>>(JsonOptions);

        page!.TotalCount.Should().Be(1);
        page.Items.Should().ContainSingle(o => o.Id == artOp.Id);
        page.Items.Should().NotContain(o => o.Id == maxOp.Id);
    }
}