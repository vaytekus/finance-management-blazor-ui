using FluentValidation;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.CreateOperationType;

public class CreateOperationTypeCommandValidator : AbstractValidator<CreateOperationTypeCommand>
{
    public CreateOperationTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Kind)
            .IsInEnum();
    }
}
