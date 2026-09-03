using FinanceManagement.Contracts.Reports;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Repositories;

public interface IReportRepository
{
    Task<IReadOnlyList<ReportByTypeAggregate>> GetByTypeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<IReadOnlyList<Operation>> GetOperationsAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
}
