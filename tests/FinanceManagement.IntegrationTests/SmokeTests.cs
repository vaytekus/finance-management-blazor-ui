using System.Net;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace FinanceManagement.IntegrationTests;

public class SmokeTests : IntegrationTestBase
{

    public SmokeTests(IntegrationTestFactory factory) : base(factory) {}

    [Fact]
    public async Task ApiStarts_AndSwaggerResponds()
    {
        var response = await Client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
