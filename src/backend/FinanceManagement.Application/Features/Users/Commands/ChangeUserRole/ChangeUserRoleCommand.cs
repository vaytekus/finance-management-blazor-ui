using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(Guid Id, string Role) : IRequest;
