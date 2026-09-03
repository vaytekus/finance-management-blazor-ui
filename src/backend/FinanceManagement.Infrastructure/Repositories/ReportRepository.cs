using FinanceManagement.Contracts.Reports;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _db;

    public ReportRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<IReadOnlyList<ReportByTypeAggregate>> GetByTypeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var raw = await _db.Operations
            .Where(o =>
                o.Wallet!.UserId == userId
                && o.Date >= from
                && o.Date <= to)
            .GroupBy(o => new
            {
                o.TypeId, o.Type!.Name, o.Type.Kind, o.Wallet!.Currency
            })
            .Select(g => new
            {
                g.Key.TypeId,
                TypeName = g.Key.Name,
                g.Key.Kind,
                g.Key.Currency,
                Total = g.Sum(x => x.Amount),
                OperationCount = g.Count()
            })
            .ToListAsync(ct);

        return raw.Select(r => new ReportByTypeAggregate
        {
            TypeId = r.TypeId,
            TypeName = r.TypeName,
            Kind = (Contracts.Enums.OperationKind)r.Kind,
            Currency = (Contracts.Enums.Currency)r.Currency,
            Total = r.Total,
            OperationCount = r.OperationCount
        }).ToList();
    }
    public async Task<IReadOnlyList<Operation>> GetOperationsAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await _db.Operations
            .Include(o => o.Type)
            .Include(o => o.Wallet)
            .Where(o => 
                o.Wallet!.UserId == userId 
                && o.Date >= from
                && o.Date <= to)
            .OrderByDescending(o => o.Date)
            .ToListAsync(ct);
            
    }
}
