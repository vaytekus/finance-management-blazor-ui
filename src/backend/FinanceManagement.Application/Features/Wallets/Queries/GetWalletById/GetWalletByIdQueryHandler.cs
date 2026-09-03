using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Queries.GetWalletById;

public class GetWalletByIdQueryHandler : IRequestHandler<GetWalletByIdQuery, WalletResponse>
{
    private readonly IWalletRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetWalletByIdQueryHandler(IWalletRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }
    
    public async Task<WalletResponse> Handle(GetWalletByIdQuery request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        return entity.ToResponse();
    }
}
