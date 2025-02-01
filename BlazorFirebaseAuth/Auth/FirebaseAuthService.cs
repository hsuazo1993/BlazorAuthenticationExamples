using System.Security.Claims;

namespace BlazorFirebaseAuth.Auth
{
    public interface IAuthService
    {
        bool IsAuthenticated();
        UserProfile? GetCurrentUser();
    }

    public class FirebaseAuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FirebaseAuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public bool IsAuthenticated()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null) return false;

            return httpContext?.User?.Identity?.IsAuthenticated == true;
        }
        public UserProfile? GetCurrentUser()
        {
            if (!IsAuthenticated()) return null;

            var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;

            if (claimsIdentity == null) return null;

            var emailClaim = claimsIdentity.FindFirst("email")?.Value;
            var emailVerifiedClaim = claimsIdentity.FindFirst("email_verified")?.Value;
            var nameClaim = claimsIdentity.FindFirst("username")?.Value;
            var givenNameClaim = claimsIdentity.FindFirst("first_name")?.Value;
            var familyNameClaim = claimsIdentity.FindFirst("last_name")?.Value;
            var pictureClaim = claimsIdentity.FindFirst("picture")?.Value;

            return new UserProfile
            {
                Email = emailClaim!,
                EmailVerified = emailVerifiedClaim != null && bool.Parse(emailVerifiedClaim),
                Name = nameClaim!,
                GivenName = givenNameClaim!,
                FamilyName = familyNameClaim!,
                Picture = pictureClaim!,
                Sub = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value!
            };
        }
    }
}
