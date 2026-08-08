using System.Security.Claims;
namespace PersonalDigitalVault.Api.Security;
public class CurrentUserService(IHttpContextAccessor accessor)
{
    public int UserId => int.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),out var id)?id:throw new UnauthorizedAccessException("User is not authenticated.");
    public string Role => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role)??string.Empty;
}
