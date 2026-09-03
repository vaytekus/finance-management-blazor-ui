using FinanceManagement.Domain.Enums;
using FluentValidation;

namespace FinanceManagement.Application.Features.Users.Commands.AdminCreateUser;

public class AdminCreateUserCommandValidator : AbstractValidator<AdminCreateUserCommand>
{
    public AdminCreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => r == UserRole.User.ToString() || r == UserRole.Admin.ToString())
            .WithMessage("Role must be 'User' or 'Admin'");
    }
}
