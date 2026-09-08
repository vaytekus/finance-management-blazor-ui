using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Application.Features.Operations.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using FinanceManagement.Domain.Entities;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Commands.CreateOperation;

public class CreateOperationCommandHandler(
    IOperationRepository repository,
    IOperationTypeRepository typeRepository,
    IWalletRepository walletRepository,
    ICurrencyConverter currencyConverter,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOperationCommand, OperationResponse>
{
    public async Task<OperationResponse> Handle(CreateOperationCommand request, CancellationToken ct)
    {
        var type = await typeRepository.GetByIdForUserAsync(request.TypeId, currentUser.Id, ct)
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

        var entity = new Operation
        {
            Id = Guid.NewGuid(),
            TypeId = request.TypeId,
            WalletId = request.WalletId,
            Amount = amount,
            Date = request.Date,
            Note = note
        };

        repository.Add(entity);
        await unitOfWork.SaveChangesAsync(ct);

        entity.Type = type;
        entity.Wallet = wallet;
        return entity.ToResponse();
    }
}
