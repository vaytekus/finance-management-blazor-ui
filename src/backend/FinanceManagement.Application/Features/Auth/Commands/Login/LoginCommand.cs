using FinanceManagement.Application.DTOs.Auth;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(string UserName, string Password) : IRequest<AuthResult>;
