using FinanceManagement.Contracts.Users;
using FinanceManagement.Application.DTOs.Auth;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse>;
