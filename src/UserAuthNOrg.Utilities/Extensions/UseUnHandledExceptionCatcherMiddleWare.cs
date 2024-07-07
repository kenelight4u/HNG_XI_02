using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using UserAuthNOrg.Utilities.Enums;

namespace UserAuthNOrg.Utilities.Extensions
{
    public class UseUnHandledExceptionCatcherMiddleWare
    {
        private readonly RequestDelegate _next;

        public UseUnHandledExceptionCatcherMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<UseUnHandledExceptionCatcherMiddleWare> logger)
        {
            //var environmentUrl = context.RequestServices.GetRequiredService<IConfiguration>()[DeploymentSettings.EnvironmentBaseUrl];
            //var isDev = environmentUrl == context.RequestServices.GetRequiredService<IConfiguration>()[DeploymentSettings.DevBaseUrl];

            try
            {
                await _next(context);
            }
            catch (CryptographicException ex)
            {
                //logger.LogError(ex, "Encryption/Decryption Failure");

                //var customMessage = "IV and Secret Key not properly formatted";

                //var errorData = isDev
                //    ? new ApiResponse<string> { Description = $"{ex.Message}", Data = ex.ToString() }
                //    : new ApiResponse<string> { Data = $"{customMessage}" };

                //var result = JsonConvert.SerializeObject(errorData);
                //context.Response.ContentType = "application/json";
                //await context.ReturnServerError(result);
            }
            catch (Exception ex)
            {
                var code = ExtensionHelper.GetCorrelationId(context);
                logger.LogError(ex, "An Unhandled Exception Occurred");
                var result = JsonConvert.SerializeObject(new ApiResponse<string>($"Something went wrong. RequestId: {code}", StatusCode.ERROR));
                context.Response.ContentType = "application/json";
                await context.ReturnServerError(result);
            }
        }
    }
}
