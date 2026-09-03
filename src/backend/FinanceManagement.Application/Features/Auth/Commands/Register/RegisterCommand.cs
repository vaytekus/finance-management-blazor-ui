using FinanceManagement.Application.DTOs.Auth;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string UserName, string Email, string Password) : IRequest<AuthResult>;
