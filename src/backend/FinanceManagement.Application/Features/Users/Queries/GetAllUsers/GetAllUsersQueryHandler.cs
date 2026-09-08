using FinanceManagement.Contracts.Users;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUserRepository repository) : IRequestHandler<GetAllUsersQuery, PagedResult<UserResponse>>
{
    public async Task<PagedResult<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var page = await repository.GetPagedAsync(request, ct);

        return page.Map(x => x.ToResponse());
    }
}
