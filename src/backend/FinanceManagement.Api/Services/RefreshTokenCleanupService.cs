using FinanceManagement.Application.Interfaces.Repositories;

namespace FinanceManagement.Api.Services;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> _logger;
    private static readonly TimeSpan _retention = TimeSpan.FromDays(30);
    private static readonly TimeSpan _interval = TimeSpan.FromHours(24);

    public RefreshTokenCleanupService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RefreshTokenCleanupService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await CleanUpAsync(ct);
            await Task.Delay(_interval, ct);
        }
    }

    private async Task CleanUpAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

            var cutoff = DateTime.UtcNow - _retention;
            var deleted = await repo.DeleteOlderThanAsync(cutoff, ct);
            _logger.LogInformation("RefreshToken cleanup: deleted {Count} tokens older than {Cutoff:O}", deleted, cutoff);
        }
        catch (Exception ex) when(ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "RefreshToken cleanup failed");
        }
    }
}
