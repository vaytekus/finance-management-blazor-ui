using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Queries.GetWalletById;

public class GetWalletByIdQueryHandler(
    IWalletRepository repository,
    ICurrentUser currentUser) : IRequestHandler<GetWalletByIdQuery, WalletResponse>
{
    public async Task<WalletResponse> Handle(GetWalletByIdQuery request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        return entity.ToResponse();
    }
}
