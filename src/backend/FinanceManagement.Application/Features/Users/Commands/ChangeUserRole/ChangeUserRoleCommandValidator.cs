using FinanceManagement.Domain.Common;
using FinanceManagement.Domain.Enums;
using FluentValidation;

namespace FinanceManagement.Application.Features.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    private static readonly string[] _allowedRoles = Enum.GetNames<UserRole>();
    
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => _allowedRoles.Contains(r))
            .WithMessage($"Role must be one of: {string.Join(", ", _allowedRoles)}");
    }
}
