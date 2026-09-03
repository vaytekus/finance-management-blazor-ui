using FluentValidation;

namespace FinanceManagement.Application.Common.Pagination;

public abstract class PagedQueryValidator<TQuery> : AbstractValidator<TQuery> where TQuery : PagedQuery
{
    protected PagedQueryValidator(string[] allowedSortFields)
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(PagedQuery.MinPage);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(PagedQuery.MinPageSize, PagedQuery.MaxPageSize);

        RuleFor(x => x.SortBy!)
            .Must(x => allowedSortFields.Contains(x.ToLowerInvariant()))
            .When(x => !string.IsNullOrEmpty(x.SortBy))
            .WithMessage($"SortBy must be one of: {string.Join(", ", allowedSortFields)}");
    }
}
