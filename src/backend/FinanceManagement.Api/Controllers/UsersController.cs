using FinanceManagement.Application.Features.Users.Commands.AdminCreateUser;
using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Features.Users.Commands.AdminResetPassword;
using FinanceManagement.Contracts.Users;
using FinanceManagement.Application.Features.Users.Commands.ChangePassword;
using FinanceManagement.Application.Features.Users.Commands.ChangeUserRole;
using FinanceManagement.Application.Features.Users.Commands.DeleteUser;
using FinanceManagement.Application.Features.Users.Commands.UpdateUserProfile;
using FinanceManagement.Application.Features.Users.Queries.GetAllUsers;
using FinanceManagement.Application.Features.Users.Queries.GetUserById;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public UsersController(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken ct)
        => Ok(await _mediator.Send(new GetUserByIdQuery(_currentUser.Id), ct));

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new UpdateUserProfileCommand(_currentUser.Id, request.UserName, request.Email), ct);
        return NoContent();
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangeMyPassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new ChangePasswordCommand(_currentUser.Id, request.CurrentPassword, request.NewPassword), ct);
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetAllUsers(
        [FromQuery] GetAllUsersQuery query,
        CancellationToken ct)
        => Ok(await _mediator.Send(query, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetUserByIdQuery(id), ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteUserCommand(id), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new ChangeUserRoleCommand(id, request.Role), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken ct
    )
    {
        await _mediator.Send(new UpdateUserProfileCommand(id, request.UserName, request.Email), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword(
        Guid id,
        [FromBody] AdminResetPasswordRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new AdminResetPasswordCommand(id, request.NewPassword), ct);
        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] AdminCreateUserRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new AdminCreateUserCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.Role), ct);
        return Ok(result);
    }
}
