using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Application.Features.OperationTypes.Commands.CreateOperationType;
using FinanceManagement.Domain.Enums;
using FinanceManagement.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.IntegrationTests.Common;

public class ValidationTests : IntegrationTestBase
{
    public ValidationTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task InvalidCommand_ReturnsProblemDetailsWith400()
    {
        // Arrange
        var user = await CreateUserAsync(UserRole.User);
        var client = await CreateAuthenticatedClientAsync(user.UserName);

        var invalidCommand = new CreateOperationTypeCommand(
            Name: "",
            Description: null,
            Kind: OperationKind.Income);

        // Act
        var response = await client.PostAsJsonAsync("/api/operation-types", invalidCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
        problem.Title.Should().Be("Bad request");
        problem.Detail.Should().Contain("Name");
    }
}