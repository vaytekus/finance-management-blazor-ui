using FluentValidation;

namespace FinanceManagement.Application.Features.Wallets.Commands.CreateWallet;

public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Currency)
            .IsInEnum();
    }
}
