using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.Features.Operations.Queries.GetAllOperations;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Repositories;

public interface IOperationRepository : IUserScopedRepository<Operation>
{
    void SoftDelete(Operation entity);
    Task<PagedResult<Operation>> GetPagedForUserAsync(Guid userId, GetAllOperationsQuery query, CancellationToken ct = default);

}
