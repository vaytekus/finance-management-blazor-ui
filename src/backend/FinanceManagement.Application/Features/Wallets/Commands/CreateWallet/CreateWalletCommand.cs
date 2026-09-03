using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.CreateWallet;

public record CreateWalletCommand(string Name, Currency Currency) : IRequest<WalletResponse>;
