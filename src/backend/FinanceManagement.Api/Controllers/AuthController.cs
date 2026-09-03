using FinanceManagement.Api.Extensions;
using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Contracts.Auth;
using FinanceManagement.Application.Features.Auth.Commands.Login;
using FinanceManagement.Application.Features.Auth.Commands.Logout;
using FinanceManagement.Application.Features.Auth.Commands.RefreshToken;
using FinanceManagement.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request.UserName, request.Email, request.Password), ct);
        return IssueTokens(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new LoginCommand(request.UserName, request.Password), ct);
        return IssueTokens(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.GetRefreshTokenCookie();

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedException("Refresh token missing.");
        }

        var result = await _mediator.Send(new RefreshTokenCommand(refreshToken), ct);
        return IssueTokens(result);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = Request.GetRefreshTokenCookie();

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _mediator.Send(new LogoutCommand(refreshToken), ct);
        }

        Response.DeleteRefreshTokenCookie(Request.IsHttps);

        return NoContent();
    }

    private ActionResult<AuthResponse> IssueTokens(AuthResult result)
    {
        Response.SetRefreshTokenCookie(result.Tokens.RefreshToken, result.Tokens.RefreshTokenExpiresAt, Request.IsHttps);
        return Ok(new AuthResponse
        {
            AccessToken = result.Tokens.AccessToken,
            ExpiresAt = result.Tokens.AccessTokenExpiresAt,
            User = result.User,
        });
    }
}
