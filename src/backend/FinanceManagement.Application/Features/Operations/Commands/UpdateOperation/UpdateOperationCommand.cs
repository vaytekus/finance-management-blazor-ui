using FinanceManagement.Application.Features.Operations.Common;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Commands.UpdateOperation;

public record UpdateOperationCommand(
    Guid Id,
    Guid TypeId,
    Guid WalletId,
    decimal Amount,
    DateTime Date,
    string? Note,
    Currency? Currency) : IOperationCommand, IRequest;
