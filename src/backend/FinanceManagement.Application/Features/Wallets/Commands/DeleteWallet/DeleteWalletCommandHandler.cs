using FinanceManagement.Application.Common;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.DeleteWallet;

public class DeleteWalletCommandHandler : IRequestHandler<DeleteWalletCommand>
{
    private readonly IWalletRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;


    public DeleteWalletCommandHandler(IWalletRepository repository, ICurrentUser currentUser, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(DeleteWalletCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        if (await _repository.HasOperationsAsync(request.Id, ct))
        {
            throw new ConflictException($"Cannot delete wallet '{entity.Name}' — it has active operations.");
        }

        _repository.SoftDelete(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
