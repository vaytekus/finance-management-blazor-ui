using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.Wallets;
using FinanceManagement.Application.Features.Wallets.Commands.CreateWallet;
using FinanceManagement.Application.Features.Wallets.Commands.DeleteWallet;
using FinanceManagement.Application.Features.Wallets.Commands.UpdateWallet;
using FinanceManagement.Application.Features.Wallets.Queries.GetAllWallets;
using FinanceManagement.Application.Features.Wallets.Queries.GetWalletById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public WalletsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<WalletResponse>>> GetAll(
        [FromQuery] GetAllWalletsQuery query,
        CancellationToken ct)
    {
        return Ok(await _mediator.Send(query, ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WalletResponse>> GetById(Guid id, CancellationToken ct)
    {
        return Ok(await _mediator.Send(new GetWalletByIdQuery(id), ct));
    }

    [HttpPost]
    public async Task<ActionResult<WalletResponse>> Create(CreateWalletCommand command, CancellationToken ct)
    {
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new
        {
            id = created.Id
        }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateWalletCommand command, CancellationToken ct)
    {
        await _mediator.Send(command with {Id = id}, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteWalletCommand(id), ct);
        return NoContent();
    }
}
