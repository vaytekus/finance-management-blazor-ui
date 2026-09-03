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

public class CreateOperationCommandHandler : IRequestHandler<CreateOperationCommand, OperationResponse>
{
    private readonly IOperationRepository _repository;
    private readonly IOperationTypeRepository _typeRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOperationCommandHandler(
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

    public async Task<OperationResponse> Handle(CreateOperationCommand request, CancellationToken ct)
    {
        var type = await _typeRepository.GetByIdForUserAsync(request.TypeId, _currentUser.Id, ct)
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

        var entity = new Operation
        {
            Id = Guid.NewGuid(),
            TypeId = request.TypeId,
            WalletId = request.WalletId,
            Amount = amount,
            Date = request.Date,
            Note = note
        };

        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        entity.Type = type;
        entity.Wallet = wallet;
        return entity.ToResponse();
    }
}
