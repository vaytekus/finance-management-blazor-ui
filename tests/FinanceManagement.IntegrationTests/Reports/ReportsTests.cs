using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Contracts.Reports;
using ContractsCurrency = FinanceManagement.Contracts.Enums.Currency;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace FinanceManagement.IntegrationTests.Reports;

public class ReportsTests : IntegrationTestBase
{
    public ReportsTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task GetDailyReport_ReturnsCorrectTotals()
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
        var incomeType = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Salary",
            Kind = OperationKind.Income, UserId = user.Id
        };
        var expenseType = new OperationType
        {
            Id = Guid.NewGuid(), Name = "Food",
            Kind = OperationKind.Expense, UserId = user.Id
        };

        var today = DateTime.UtcNow;
        var op1 = new Operation
        {
            Id = Guid.NewGuid(), TypeId = incomeType.Id, WalletId = wallet.Id,
            Amount = 100m, Date = today
        };
        var op2 = new Operation
        {
            Id = Guid.NewGuid(), TypeId = incomeType.Id, WalletId = wallet.Id,
            Amount = 200m, Date = today
        };
        var op3 = new Operation
        {
            Id = Guid.NewGuid(), TypeId = expenseType.Id, WalletId = wallet.Id,
            Amount = 50m, Date = today
        };

        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(wallet);
            db.OperationTypes.AddRange(incomeType, expenseType);
            db.Operations.AddRange(op1, op2, op3);
            await db.SaveChangesAsync();
        });

        // Act
        var response = await client.GetAsync(
            $"/api/reports/daily?date={today:yyyy-MM-dd}&currency=UAH");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var report = await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions);

        report!.Currency.Should().Be(ContractsCurrency.UAH);
        report.TotalIncome.Should().Be(300m);
        report.TotalExpense.Should().Be(50m);
        report.TotalBalance.Should().Be(250m);
        report.Operations.Should().HaveCount(3);
        report.ByType.Should().HaveCount(2);
        report.ByType.Single(x => x.TypeName == "Salary").Total.Should().Be(300m);
        report.ByType.Single(x => x.TypeName == "Salary").OperationCount.Should().Be(2);
        report.ByType.Single(x => x.TypeName == "Food").Total.Should().Be(50m);
    }

    [Fact]
    public async Task GetDailyReport_ExcludesOperationsOutsideDate()
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
            Id = Guid.NewGuid(), Name = "Salary",
            Kind = OperationKind.Income, UserId = user.Id
        };

        var today = DateTime.UtcNow.Date.AddHours(12);
        var yesterday = today.AddDays(-1);

        var todayOp = new Operation
        {
            Id = Guid.NewGuid(), TypeId = type.Id, WalletId = wallet.Id,
            Amount = 100m, Date = today
        };
        var yesterdayOp = new Operation
        {
            Id = Guid.NewGuid(), TypeId = type.Id, WalletId = wallet.Id,
            Amount = 999m, Date = yesterday
        };

        await ExecuteDbAsync(async db =>
        {
            db.Wallets.Add(wallet);
            db.OperationTypes.Add(type);
            db.Operations.AddRange(todayOp, yesterdayOp);
            await db.SaveChangesAsync();
        });

        // Act
        var response = await client.GetAsync(
            $"/api/reports/daily?date={today:yyyy-MM-dd}&currency=UAH");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var report = await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions);

        report!.TotalIncome.Should().Be(100m);
        report.Operations.Should().HaveCount(1);
        report.Operations.Should().ContainSingle(o => o.Id == todayOp.Id);
        report.Operations.Should().NotContain(o => o.Id == yesterdayOp.Id);
    }

    [Fact]
    public async Task GetPeriodReport_ExcludesOtherUsersOperations()
    {
        // Arrange
        var art = await CreateUserAsync(UserRole.User, userName: "art");
        var max = await CreateUserAsync(UserRole.User, userName: "max");

        var artWallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Art W", Currency = Currency.UAH,
            UserId = art.Id, CreatedAt = DateTime.UtcNow
        };
        var maxWallet = new Wallet
        {
            Id = Guid.NewGuid(),
            Name = "Max W", Currency = Currency.UAH,
            UserId = max.Id, CreatedAt = DateTime.UtcNow
        };
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

        var day = DateTime.UtcNow.Date.AddHours(12);
        var artOp = new Operation
        {
            Id = Guid.NewGuid(), TypeId = artType.Id, WalletId = artWallet.Id,
            Amount = 100m, Date = day
        };
        var maxOp = new Operation
        {
            Id = Guid.NewGuid(), TypeId = maxType.Id, WalletId = maxWallet.Id,
            Amount = 500m, Date = day
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
        var from = day.AddDays(-1);
        var to = day.AddDays(1);
        var response = await artClient.GetAsync(
            $"/api/reports/period?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&currency=UAH");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var report = await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions);

        report!.TotalIncome.Should().Be(100m);
        report.Operations.Should().HaveCount(1);
        report.Operations.Should().ContainSingle(o => o.Id == artOp.Id);
        report.Operations.Should().NotContain(o => o.Id == maxOp.Id);
        report.ByType.Should().ContainSingle(x => x.TypeName == "Art Salary");
        report.ByType.Should().NotContain(x => x.TypeName == "Max Salary");
    }
}