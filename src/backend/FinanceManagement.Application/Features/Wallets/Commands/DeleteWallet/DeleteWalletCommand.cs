using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.DeleteWallet;

public record DeleteWalletCommand(Guid Id) : IRequest;
