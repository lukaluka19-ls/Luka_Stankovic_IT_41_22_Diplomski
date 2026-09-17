using System.Security.Claims;
using Igraonica.Application.Abstractions;
using Igraonica.Domain.Enums;

namespace Igraonica.WebAPI.Services;

public class CurrentUserService(IHttpContextAccessor _httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return value is not null && Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public UserRole Role
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.Role);
            return value is not null && Enum.TryParse<UserRole>(value, out var role) ? role : UserRole.Guest;
        }
    }
}
