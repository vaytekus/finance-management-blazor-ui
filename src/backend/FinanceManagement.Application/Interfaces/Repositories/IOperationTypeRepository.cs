using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.Features.OperationTypes.Queries.GetAllOperationTypes;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Repositories;

public interface IOperationTypeRepository : IUserScopedRepository<OperationType>
{
    Task<bool> ExistsByNameForUserAsync(string name, Guid userId, Guid? excludeId = null, CancellationToken ct = default);
    Task<bool> IsUsedInOperationsAsync(Guid typeId, CancellationToken ct = default);

    Task<int> CountOperationsAsync(Guid typeId, CancellationToken ct = default);
    Task ReassignOperationsAsync(Guid fromTypeId, Guid toTypeId, CancellationToken ct = default);

    void Delete(OperationType entity);

    Task<PagedResult<OperationType>> GetPagedForUserAsync(
        Guid userId,
        GetAllOperationTypesQuery query,
        CancellationToken ct = default);
}
