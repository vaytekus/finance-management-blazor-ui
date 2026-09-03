using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Queries.GetAllWallets;

public class GetAllWalletsQueryHandler : IRequestHandler<GetAllWalletsQuery, PagedResult<WalletResponse>>
{
    private readonly IWalletRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetAllWalletsQueryHandler(IWalletRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }
    
    public async Task<PagedResult<WalletResponse>> Handle(GetAllWalletsQuery request, CancellationToken ct)
    {
        var page = await _repository.GetPagedForUserAsync(_currentUser.Id, request, ct);

        return page.Map(x => x.ToResponse());
    }
}
