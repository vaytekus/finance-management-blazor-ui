using System.Security.Claims;
using FinanceManagement.Application.Interfaces.Services;

namespace FinanceManagement.Api.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid Id
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("Current user is not authenticated.");
            return Guid.Parse(value);
        }
    }
}
