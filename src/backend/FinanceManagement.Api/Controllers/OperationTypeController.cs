using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Application.Features.OperationTypes.Commands.CreateOperationType;
using FinanceManagement.Application.Features.OperationTypes.Commands.DeleteOperationType;
using FinanceManagement.Application.Features.OperationTypes.Commands.UpdateOperationType;
using FinanceManagement.Application.Features.OperationTypes.Queries.GetAllOperationTypes;
using FinanceManagement.Application.Features.OperationTypes.Queries.GetOperationTypeById;
using FinanceManagement.Application.Features.OperationTypes.Queries.GetOperationTypeOperationsCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/operation-types")]
public class OperationTypeController : ControllerBase
{
    private readonly IMediator _mediator;

    public OperationTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OperationTypeResponse>>> Get(
        [FromQuery] GetAllOperationTypesQuery query,
        CancellationToken ct)
        => Ok(await _mediator.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OperationTypeResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetOperationTypeByIdQuery(id), ct));

    [HttpGet("{id:guid}/operations-count")]
    public async Task<ActionResult<int>> GetOperationsCount(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetOperationTypeOperationsCountQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<OperationTypeResponse>> Create(
        [FromBody] CreateOperationTypeCommand command,
        CancellationToken ct)
    {
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateOperationTypeCommand command,
        CancellationToken ct)
    {
        await _mediator.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromBody] DeleteOperationTypeRequest? body,
        CancellationToken ct)
    {
        await _mediator.Send(new DeleteOperationTypeCommand(id, body?.ReplaceWithId, body?.ReplaceWithName), ct);
        return NoContent();
    }
}
