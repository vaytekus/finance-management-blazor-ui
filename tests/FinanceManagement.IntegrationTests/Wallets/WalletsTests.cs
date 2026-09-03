using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.Wallets;
using ContractsCurrency = FinanceManagement.Contracts.Enums.Currency;
using FinanceManagement.Application.Features.Wallets.Commands.CreateWallet;
using FinanceManagement.Application.Features.Wallets.Commands.UpdateWallet;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.IntegrationTests.Wallets;

public class WalletsTests : IntegrationTestBase
{
    public WalletsTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateWallet_ReturnsCreated_AndPersistsInDb()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var command = new CreateWalletCommand("Savings", Currency.USD);

        // Act
        var response = await client.PostAsJsonAsync("/api/wallets", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<WalletResponse>(JsonOptions);
        created!.Name.Should().Be("Savings");
        created.Currency.Should().Be(ContractsCurrency.USD);
        created.Id.Should().NotBeEmpty();

        var inDb = await ExecuteDbAsync(db =>
            db.Wallets.SingleOrDefaultAsync(w => w.Id == created.Id));

        inDb.Should().NotBeNull();
        inDb.UserId.Should().Be(user.Id);
        inDb.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task GetMyWallets_ReturnsOnlyOwnedByCurrentUser()
    {
        // Arrange
        var art = await CreateUserAsync(UserRole.User, userName: "art");
        var max = await CreateUserAsync(UserRole.User, userName: "max");

        var artWallet = new Wallet
        {
            Name = "Art Cash", Currency = Currency.UAH,
            UserId = art.Id, CreatedAt = DateTime.UtcNow
        };
        var maxWallet = new Wallet
        {
            Name = "Max Savings", Currency = Currency.EUR,
            UserId = max.Id, CreatedAt = DateTime.UtcNow
        };

        await ExecuteDbAsync(async db =>
        {
            db.Wallets.AddRange(artWallet, maxWallet);
            await db.SaveChangesAsync();
        });

        var artClient = await CreateAuthenticatedClientAsync(art.UserName);

        // Act
        var response = await artClient.GetAsync("/api/wallets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var page = await response.Content.ReadFromJsonAsync<PagedResult<WalletResponse>>(JsonOptions);

        page!.TotalCount.Should().Be(1);
        page.Items.Should().ContainSingle(w => w.Id == artWallet.Id);
        page.Items.Should().NotContain(w => w.Id == maxWallet.Id);
    }

    [Fact]
    public async Task GetWalletById_ForForeignWallet_Returns404()
    {
        // Arrange
        var art = await CreateUserAsync(UserRole.User, userName: "art");
        var max = await CreateUserAsync(UserRole.User, userName: "max");

        var maxWallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Max Wallet", Currency = Currency.USD,
            UserId = max.Id, CreatedAt = DateTime.UtcNow
        };
        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(maxWallet);
            await db.SaveChangesAsync();
        });

        var artClient = await CreateAuthenticatedClientAsync(art.UserName);

        // Act
        var response = await artClient.GetAsync($"/api/wallets/{maxWallet.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateWallet_UpdatesNameAndCurrency_InDb()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Old Name", Currency = Currency.UAH,
            UserId = user.Id, CreatedAt = DateTime.UtcNow
        };
        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(wallet);
            await db.SaveChangesAsync();
        });

        var command = new UpdateWalletCommand(Id: Guid.Empty, Name: "New Name", Currency: Currency.EUR);

        // Act
        var response = await client.PutAsJsonAsync($"/api/wallets/{wallet.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var inDb = await ExecuteDbAsync(db =>
            db.Wallets.SingleAsync(w => w.Id == wallet.Id));
        inDb.Name.Should().Be("New Name");
        inDb.Currency.Should().Be(Currency.EUR);
    }

    [Fact]
    public async Task DeleteWallet_SetsDeletedAt_AndHidesFromGetAll()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var wallet = new Wallet
        {
            Name = "To Delete", Currency = Currency.UAH,
            UserId = user.Id, CreatedAt = DateTime.UtcNow
        };
        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(wallet);
            await db.SaveChangesAsync();
        });

        // Act
        var deleteResponse = await client.DeleteAsync($"/api/wallets/{wallet.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var inDb = await ExecuteDbAsync(db =>
            db.Wallets.IgnoreQueryFilters()
                .SingleOrDefaultAsync(w => w.Id == wallet.Id));

        inDb.Should().NotBeNull();
        inDb.DeletedAt.Should().NotBeNull();
        inDb.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var getResponse = await client.GetAsync("/api/wallets");
        var page = await getResponse.Content.ReadFromJsonAsync<PagedResult<WalletResponse>>(JsonOptions);
        page!.Items.Should().NotContain(w => w.Id == wallet.Id);
    }
}