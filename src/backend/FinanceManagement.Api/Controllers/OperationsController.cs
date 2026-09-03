using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Application.Features.Operations.Commands.CreateOperation;
using FinanceManagement.Application.Features.Operations.Commands.DeleteOperation;
using FinanceManagement.Application.Features.Operations.Commands.UpdateOperation;
using FinanceManagement.Application.Features.Operations.Queries.GetAllOperations;
using FinanceManagement.Application.Features.Operations.Queries.GetOperationById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/operations")]
public class OperationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OperationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OperationResponse>>> Get(
        [FromQuery] GetAllOperationsQuery query,
        CancellationToken ct)
        => Ok(await _mediator.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OperationResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetOperationByIdQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<OperationResponse>> Create(
        [FromBody] CreateOperationCommand command,
        CancellationToken ct)
    {
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateOperationCommand command,
        CancellationToken ct)
    {
        await _mediator.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteOperationCommand(id), ct);
        return NoContent();
    }
}
