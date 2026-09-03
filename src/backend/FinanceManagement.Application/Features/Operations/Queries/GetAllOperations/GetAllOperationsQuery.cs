using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Queries.GetAllOperations;

public record GetAllOperationsQuery : PagedQuery, IRequest<PagedResult<OperationResponse>>
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? WalletId { get; set; }
    public Guid? TypeId { get; set; }
    public decimal? AmountMin { get; set; }
    public decimal? AmountMax { get; set; }
    public string? Search { get; set; }
    public Currency? Currency { get; set; }
}
