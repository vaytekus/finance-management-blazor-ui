using FinanceManagement.Application.Common;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.DeleteWallet;

public class DeleteWalletCommandHandler(
    IWalletRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteWalletCommand>
{
    public async Task Handle(DeleteWalletCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        if (await repository.HasOperationsAsync(request.Id, ct))
        {
            throw new ConflictException($"Cannot delete wallet '{entity.Name}' — it has active operations.");
        }

        repository.SoftDelete(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
