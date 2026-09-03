using FinanceManagement.Application.Features.Reports.Queries.GetPeriodReport;
using FinanceManagement.Domain.Enums;
using FluentAssertions;

namespace FinanceManagement.Tests.Features.Reports;

public class GetPeriodReportQueryValidatorTests
{
    private readonly GetPeriodReportQueryValidator _sut = new();

    [Fact]
    public void Validate_WhenFromGreaterThanTo_Invalid()
    {
        var query = new GetPeriodReportQuery(
            From: new DateTime(2026, 8, 10),
            To: new DateTime(2026, 8, 1),
            Currency: Currency.UAH);

        var result = _sut.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Match("*must be less than or equal to*");
    }

    [Fact]
    public void Validate_WhenFromEqualsOrLessThanTo_Valid()
    {
        var query = new GetPeriodReportQuery(
            From: new DateTime(2026, 8, 1),
            To: new DateTime(2026, 8, 10),
            Currency: Currency.UAH);

        var result = _sut.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}