using FinanceManagement.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManagement.Functions.Functions;

public class DailyStatsFunction
{
    private readonly AppDbContext _db;
    private readonly ILogger<DailyStatsFunction> _logger;

    public DailyStatsFunction(
        AppDbContext db,
        ILogger<DailyStatsFunction> logger)
    {
        _db = db;
        _logger = logger;
    }

    [Function("DailyStats")]
    public async Task Run(
        [TimerTrigger("0 0 6 * * *")] TimerInfo timer,
        CancellationToken ct)
    {
        var users = await _db.Users.CountAsync(ct);
        var wallets = await _db.Wallets.CountAsync(ct);
        var operations = await _db.Operations.CountAsync(ct);
        
        _logger.LogInformation(
            "Daily stats — Users: {Users}, Wallets: {Wallets}, Operations: {Operations}",
            users, wallets, operations); 
    }
}
