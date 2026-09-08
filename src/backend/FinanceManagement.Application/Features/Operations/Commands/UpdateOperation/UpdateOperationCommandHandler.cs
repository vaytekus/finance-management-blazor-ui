using FinanceManagement.Application.Common;
using FinanceManagement.Application.Features.Operations.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Commands.UpdateOperation;

public class UpdateOperationCommandHandler(
    IOperationRepository repository,
    IOperationTypeRepository typeRepository,
    IWalletRepository walletRepository,
    ICurrencyConverter currencyConverter,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateOperationCommand>
{
    public async Task Handle(UpdateOperationCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        await typeRepository.GetByIdForUserAsync(request.TypeId, currentUser.Id, ct)
            .OrThrowAsync(request.TypeId);

        var wallet = await walletRepository.GetByIdForUserAsync(request.WalletId, currentUser.Id, ct)
            .OrThrowAsync(request.WalletId);

        var (amount, note) = await OperationAmountResolver.ResolveAsync(
            currencyConverter,
            request.Amount,
            request.Currency,
            request.Note,
            request.Date,
            wallet.Currency,
            ct);

        entity.TypeId = request.TypeId;
        entity.WalletId = request.WalletId;
        entity.Amount = amount;
        entity.Date = request.Date;
        entity.Note = note;

        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
