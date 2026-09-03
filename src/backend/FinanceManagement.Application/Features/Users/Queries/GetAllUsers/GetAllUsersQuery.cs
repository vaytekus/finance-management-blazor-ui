using FinanceManagement.Contracts.Users;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : PagedQuery, IRequest<PagedResult<UserResponse>>
{
    public string? Search { get; set; }
    public string? Role { get; set; }
}
