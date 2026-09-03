using FluentValidation;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.UpdateOperationType;

public class UpdateOperationTypeCommandValidator : AbstractValidator<UpdateOperationTypeCommand>
{
    public UpdateOperationTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Kind)
            .IsInEnum();
    }
}
