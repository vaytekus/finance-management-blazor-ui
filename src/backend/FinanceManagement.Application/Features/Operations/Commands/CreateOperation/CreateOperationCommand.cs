using FinanceManagement.Contracts.Operations;
using FinanceManagement.Application.Features.Operations.Common;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Commands.CreateOperation;

public record CreateOperationCommand(
    Guid TypeId,
    Guid WalletId,
    decimal Amount,
    DateTime Date,
    string? Note,
    Currency? Currency) : IOperationCommand, IRequest<OperationResponse>;
