using FinanceManagement.Application.Common;
using FluentValidation;

namespace FinanceManagement.Application.Features.Operations.Common;

public abstract class OperationCommandValidatorBase<T> : AbstractValidator<T>
    where T : IOperationCommand
{
    protected OperationCommandValidatorBase()
    {
        RuleFor(x => x.TypeId)
            .NotEmpty();

        RuleFor(x => x.WalletId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo((decimal)ValidationConstants.MinOperationAmount);

        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.Note)
            .MaximumLength(500);

        RuleFor(x => x.Currency)
            .IsInEnum()
            .When(x => x.Currency.HasValue);
    }
}
