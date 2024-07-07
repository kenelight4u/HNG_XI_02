using Microsoft.AspNetCore.Http;
using UserAuthNOrg.Utilities.Enums;

namespace UserAuthNOrg.Utilities.Extensions
{
    public static class ContextHelper
    {
        private static Task ReturnError(this HttpContext context, string msg, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(msg);
        }

        public static Task ReturnBadRequest(this HttpContext context, string message) => context.ReturnError(message, StatusCodes.Status400BadRequest);

        public static Task ReturnUnauthorized(this HttpContext context, string message) => context.ReturnError(message, StatusCodes.Status401Unauthorized);

        public static Task ReturnForbidden(this HttpContext context, string message) => context.ReturnError(message, StatusCodes.Status403Forbidden);

        public static Task ReturnServerError(this HttpContext context, string message) => context.ReturnError(message, (int)StatusCode.ERROR);
    }
}
