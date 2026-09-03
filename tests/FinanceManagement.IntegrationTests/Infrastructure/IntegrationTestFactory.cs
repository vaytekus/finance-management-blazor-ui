using System.Data.Common;
using FinanceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace FinanceManagement.IntegrationTests.Infrastructure;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("finance_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    private DbConnection _connection = null!;
    private Respawner _respawner = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("Jwt__Key", "test-signing-key-must-be-at-least-32-chars-long-for-hmac-sha256");
        Environment.SetEnvironmentVariable("SeedAdmin__Password", "admin");
        
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }
        
        _connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _connection.OpenAsync();
        
        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] {"public"},
            TablesToIgnore = new Table[] { "__EFMigrationsHistory", "Roles" }
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connection);
    }
    
    public new async Task DisposeAsync() {
        await _connection.DisposeAsync();
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
