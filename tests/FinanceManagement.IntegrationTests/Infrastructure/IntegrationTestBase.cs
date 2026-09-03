using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceManagement.Contracts.Auth;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using FinanceManagement.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManagement.IntegrationTests.Infrastructure;

[Collection(nameof(IntegrationTestCollection))]
public class IntegrationTestBase : IAsyncLifetime
{
    protected readonly IntegrationTestFactory Factory;
    protected readonly HttpClient Client;
    public const string DefaultPassword = "P@ssw0rd";

    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public IntegrationTestBase(IntegrationTestFactory factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
    }
    
    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }
    
    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task<T> ExecuteDbAsync<T>(Func<AppDbContext, Task<T>> action)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await action(db);
    }
    
    protected async Task ExecuteDbAsync(Func<AppDbContext, Task> action)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await action(db);
    }

    protected async Task<User> CreateUserAsync(
        UserRole role,
        string? userName = null,
        string? email = null,
        string password = DefaultPassword)
    {
        userName ??= $"u_{Guid.NewGuid():N}"[..10];
        email ??= $"{userName}@test.local";

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var user = new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = hasher.Hash(password),
            RoleId = role,
            CreatedAt = DateTime.UtcNow
        };
        
        db.Users.Add(user);
        await  db.SaveChangesAsync();
        return user;
    }

    protected async Task<string> LoginAsync(string userName, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new
            {
                UserName = userName, Password = password
            });
        response.EnsureSuccessStatusCode();
        
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.AccessToken;
    }

    protected async Task<HttpClient> CreateAuthenticatedClientAsync(
        string userName,
        string password = DefaultPassword)
    {
        var token = await LoginAsync(userName, password);
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
