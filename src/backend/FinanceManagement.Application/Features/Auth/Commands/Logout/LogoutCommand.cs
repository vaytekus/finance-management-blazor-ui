using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;
