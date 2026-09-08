using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using FinanceManagement.Domain.Entities;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.CreateWallet;

public class CreateWalletCommandHandler(
    IWalletRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateWalletCommand, WalletResponse>
{
    public async Task<WalletResponse> Handle(CreateWalletCommand request, CancellationToken ct)
    {
        await repository.ExistsByNameForUserAsync(request.Name, currentUser.Id, excludeId: null, ct)
            .ThrowIfExistsAsync($"Wallet with name '{request.Name}' already exists.");

        var entity = new Wallet
        {
            Name = request.Name, Currency = request.Currency, UserId = currentUser.Id, CreatedAt = DateTime.UtcNow
        };

        repository.Add(entity);
        await unitOfWork.SaveChangesAsync(ct);
        return entity.ToResponse();
    }
}
