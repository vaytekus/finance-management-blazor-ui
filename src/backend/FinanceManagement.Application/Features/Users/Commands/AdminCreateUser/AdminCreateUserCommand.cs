using FinanceManagement.Contracts.Users;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.AdminCreateUser;

public record AdminCreateUserCommand(
    string UserName,
    string Email,
    string Password,
    string Role) : IRequest<UserResponse>;
