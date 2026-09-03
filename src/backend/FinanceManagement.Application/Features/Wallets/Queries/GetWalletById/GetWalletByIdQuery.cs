using FinanceManagement.Contracts.Wallets;
using MediatR;

namespace FinanceManagement.Application.Features.Wallets.Queries.GetWalletById;

public record GetWalletByIdQuery(Guid Id) : IRequest<WalletResponse>;
