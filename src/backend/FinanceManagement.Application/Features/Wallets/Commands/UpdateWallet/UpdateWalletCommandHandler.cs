using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.UpdateWallet;

public class UpdateWalletCommandHandler(
    IWalletRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateWalletCommand>
{
    public async Task Handle(UpdateWalletCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        await repository.ExistsByNameForUserAsync(request.Name, currentUser.Id, excludeId: request.Id, ct)
            .ThrowIfExistsAsync($"Wallet with name '{request.Name}' already exists.");

        entity.Name = request.Name;
        entity.Currency = request.Currency;

        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
