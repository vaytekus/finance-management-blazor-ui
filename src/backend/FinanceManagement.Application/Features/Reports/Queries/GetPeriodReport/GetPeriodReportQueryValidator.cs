using FluentValidation;

namespace FinanceManagement.Application.Features.Reports.Queries.GetPeriodReport;

public class GetPeriodReportQueryValidator : AbstractValidator<GetPeriodReportQuery>
{
    public GetPeriodReportQueryValidator()
    {
        RuleFor(x => x.From)
            .LessThanOrEqualTo(x => x.To)
            .WithMessage(x => $"'from' ({x.From:yyyy-MM-dd}) must be less than or equal to 'to' ({x.To:yyyy-MM-dd}).");
    }
}
