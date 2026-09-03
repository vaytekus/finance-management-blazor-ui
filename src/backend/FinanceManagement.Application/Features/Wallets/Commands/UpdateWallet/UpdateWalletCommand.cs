using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Commands.UpdateWallet;

public record UpdateWalletCommand(Guid Id, string Name, Currency Currency) : IRequest;
