using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using FinanceManagement.Domain.Entities;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.CreateWallet;

public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, WalletResponse>
{
    private readonly IWalletRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWalletCommandHandler(IWalletRepository repository, ICurrentUser currentUser, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<WalletResponse> Handle(CreateWalletCommand request, CancellationToken ct)
    {
        await _repository.ExistsByNameForUserAsync(request.Name, _currentUser.Id, excludeId: null, ct)
            .ThrowIfExistsAsync($"Wallet with name '{request.Name}' already exists.");

        var entity = new Wallet
        {
            Name = request.Name, Currency = request.Currency, UserId = _currentUser.Id, CreatedAt = DateTime.UtcNow
        };

        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(ct);
        return entity.ToResponse();
    }
}
