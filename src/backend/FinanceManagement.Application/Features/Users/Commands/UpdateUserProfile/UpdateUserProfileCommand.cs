using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(Guid UserId, string UserName, string Email) : IRequest;
