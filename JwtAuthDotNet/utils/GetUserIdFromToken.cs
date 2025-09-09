using System.Security.Claims;

namespace JwtAuthDotNet.utils
{
    public class GetUserIdFromToken
    {
        public static Guid ExtractUserId(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Guid.Empty; // or throw an exception based on your error handling strategy
            }
            return userId;
        }
    }
}
