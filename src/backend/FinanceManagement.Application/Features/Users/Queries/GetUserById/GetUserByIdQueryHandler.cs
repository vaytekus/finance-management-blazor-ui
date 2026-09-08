using FinanceManagement.Contracts.Users;
using FinanceManagement.Application.Common;
using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserRepository repository) : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct).OrThrowAsync(request.Id);
        return user.ToResponse();
    }
}
