using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Queries.GetAllWallets;

public class GetAllWalletsQueryHandler(
    IWalletRepository repository,
    ICurrentUser currentUser) : IRequestHandler<GetAllWalletsQuery, PagedResult<WalletResponse>>
{
    public async Task<PagedResult<WalletResponse>> Handle(GetAllWalletsQuery request, CancellationToken ct)
    {
        var page = await repository.GetPagedForUserAsync(currentUser.Id, request, ct);

        return page.Map(x => x.ToResponse());
    }
}
