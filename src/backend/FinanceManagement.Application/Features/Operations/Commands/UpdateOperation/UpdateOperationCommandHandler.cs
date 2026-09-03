using FinanceManagement.Application.Common;
using FinanceManagement.Application.Features.Operations.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Commands.UpdateOperation;

public class UpdateOperationCommandHandler : IRequestHandler<UpdateOperationCommand>
{
    private readonly IOperationRepository _repository;
    private readonly IOperationTypeRepository _typeRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOperationCommandHandler(
        IOperationRepository repository,
        IOperationTypeRepository typeRepository,
        IWalletRepository walletRepository,
        ICurrencyConverter currencyConverter,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _typeRepository = typeRepository;
        _walletRepository = walletRepository;
        _currencyConverter = currencyConverter;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateOperationCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        await _typeRepository.GetByIdForUserAsync(request.TypeId, _currentUser.Id, ct)
            .OrThrowAsync(request.TypeId);

        var wallet = await _walletRepository.GetByIdForUserAsync(request.WalletId, _currentUser.Id, ct)
            .OrThrowAsync(request.WalletId);

        var (amount, note) = await OperationAmountResolver.ResolveAsync(
            _currencyConverter,
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

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
