using FinanceManagement.Application.DTOs.Auth;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;
