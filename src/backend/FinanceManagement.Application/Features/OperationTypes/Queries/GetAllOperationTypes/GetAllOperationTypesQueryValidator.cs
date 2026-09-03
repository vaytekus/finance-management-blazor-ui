using FinanceManagement.Application.Common.Pagination;
using FluentValidation;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetAllOperationTypes;

public class GetAllOperationTypesQueryValidator : PagedQueryValidator<GetAllOperationTypesQuery>
{
    private static readonly string[] _allowedSortFields = ["name", "kind"];

    public GetAllOperationTypesQueryValidator() : base(_allowedSortFields)
    {}
}