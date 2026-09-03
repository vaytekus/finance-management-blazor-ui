using FinanceManagement.Application.Common.Pagination;
using FluentValidation;

namespace FinanceManagement.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryValidator : PagedQueryValidator<GetAllUsersQuery>
{
    private static readonly string[] _allowedSortFields = ["username", "email", "role", "createdat"];

    public GetAllUsersQueryValidator() : base(_allowedSortFields)
    {}
}