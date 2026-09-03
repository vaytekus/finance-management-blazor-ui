using FluentValidation;

namespace FinanceManagement.Application.Features.Users.Commands.AdminResetPassword;

public class AdminResetPasswordCommandValidator : AbstractValidator<AdminResetPasswordCommand>
{
    public  AdminResetPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);
    }
}
