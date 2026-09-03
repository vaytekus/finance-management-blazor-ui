using FinanceManagement.Application.Features.Operations.Common;
using FluentValidation;

namespace FinanceManagement.Application.Features.Operations.Commands.UpdateOperation;

public class UpdateOperationCommandValidator : OperationCommandValidatorBase<UpdateOperationCommand>
{
    public UpdateOperationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
