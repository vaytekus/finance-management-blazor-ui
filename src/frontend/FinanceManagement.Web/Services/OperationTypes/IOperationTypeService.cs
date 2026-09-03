using FinanceManagement.Contracts.OperationTypes;

namespace FinanceManagement.Web.Services.OperationTypes;

public interface IOperationTypeService
{
    Task<IReadOnlyList<OperationTypeResponse>> GetAllAsync(CancellationToken ct = default);
    Task<OperationTypeResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OperationTypeResponse> CreateAsync(CreateOperationTypeRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateOperationTypeRequest request, CancellationToken ct = default);
    Task<int> CountOperationsAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, DeleteOperationTypeRequest? body = null, CancellationToken ct = default);
}
