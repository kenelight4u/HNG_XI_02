using Microsoft.AspNetCore.Http;
using UserAuthNOrg.Utilities.CoreConstants;

namespace UserAuthNOrg.Utilities.Extensions
{
    public class HeaderHandler
    {
        private readonly RequestDelegate _next;

        public HeaderHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            if (string.IsNullOrEmpty(ExtensionHelper.GetClientId(context)))
            {
                var clientId = Guid.NewGuid().ToString();
                context.Request.Headers.Append(ConstantsString.ClientId, clientId);
            }


            if (string.IsNullOrEmpty(ExtensionHelper.GetCorrelationId(context)))
            {
                var correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Append(ConstantsString.CorrelationId, correlationId);
            }

            await _next(context);
        }

       
    }
}
