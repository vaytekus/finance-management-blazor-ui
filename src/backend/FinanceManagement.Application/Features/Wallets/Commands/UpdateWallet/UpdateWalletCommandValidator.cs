using FluentValidation;

namespace FinanceManagement.Application.Features.Wallets.Commands.UpdateWallet;

public class UpdateWalletCommandValidator : AbstractValidator<UpdateWalletCommand>
{
    public UpdateWalletCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Currency)
            .IsInEnum();
    }
}
