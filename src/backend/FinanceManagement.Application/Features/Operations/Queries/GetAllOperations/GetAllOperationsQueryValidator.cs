using FinanceManagement.Application.Common.Pagination;
using FluentValidation;

namespace FinanceManagement.Application.Features.Operations.Queries.GetAllOperations;

public class GetAllOperationsQueryValidator : PagedQueryValidator<GetAllOperationsQuery>
{
    private static readonly string[] _allowedSortFields = ["date", "amount", "note"];

    public GetAllOperationsQueryValidator() : base(_allowedSortFields)
    {
        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom!.Value)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
    }
}