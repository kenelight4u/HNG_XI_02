using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UserAuthNOrg.Infrastructure.Interfaces;

namespace UserAuthNOrg.Infrastructure.Services
{
    public class ContextAccessor : IContextAccessor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public ContextAccessor(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetCurrentUserId()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            // Gets list of claims.
            var claim = identity.Claims;

            // Gets userId from claims as string.
            var userIdClaim = claim
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return userIdClaim?.Value;
        }

        public string GetCurrentUserEmail()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            // Gets list of claims.
            var claim = identity.Claims;

            // Gets user email from claims. Generally it's a  string.
            var loggedInUSerEmail = claim
                .First(x => x.Type == ClaimTypes.Email).Value;

            return loggedInUSerEmail;
        }
    }
}
