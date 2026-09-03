using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetAllOperationTypes;

public record GetAllOperationTypesQuery : PagedQuery, IRequest<PagedResult<OperationTypeResponse>>
{
    public string? Search { get; set; }
    public OperationKind? Kind { get; set; }
}