using Microsoft.AspNetCore.Builder;

namespace UserAuthNOrg.Utilities.Extensions
{
    public static class ExtensionHandlers
    {
        public static void UseHeaderHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<HeaderHandler>();
        }

        public static IApplicationBuilder UseUnHandledExceptionCatcher(this IApplicationBuilder app)
            => app.UseMiddleware<UseUnHandledExceptionCatcherMiddleWare>();
    }
}
