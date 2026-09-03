using FinanceManagement.Contracts.Common;
using FinanceManagement.Contracts.Operations;

namespace FinanceManagement.Web.Services.Operations;

public interface IOperationService
{
    Task<PagedResult<OperationResponse>> GetAsync(OperationQuery query, CancellationToken ct = default);
    Task<OperationResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OperationResponse> CreateAsync(CreateOperationRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateOperationRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
