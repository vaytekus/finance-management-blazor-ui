using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.AdminResetPassword;

public record AdminResetPasswordCommand(Guid UserId, string NewPassword) : IRequest;
