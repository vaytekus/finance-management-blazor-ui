using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Queries.GetAllWallets;

public record GetAllWalletsQuery : PagedQuery, IRequest<PagedResult<WalletResponse>>
{
    public string? Search { get; set; }
    public Currency? Currency { get; set; }
}
