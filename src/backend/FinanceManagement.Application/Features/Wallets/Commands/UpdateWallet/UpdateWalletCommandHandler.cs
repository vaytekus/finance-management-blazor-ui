using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.UpdateWallet;

public class UpdateWalletCommandHandler : IRequestHandler<UpdateWalletCommand>
{
    private readonly IWalletRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateWalletCommandHandler(IWalletRepository repository, ICurrentUser currentUser, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(UpdateWalletCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);
        
        await _repository.ExistsByNameForUserAsync(request.Name, _currentUser.Id, excludeId: request.Id, ct)
            .ThrowIfExistsAsync($"Wallet with name '{request.Name}' already exists.");
        
        entity.Name = request.Name;
        entity.Currency = request.Currency;
        
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
