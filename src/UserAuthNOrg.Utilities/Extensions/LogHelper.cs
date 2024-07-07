using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace UserAuthNOrg.Utilities.Extensions
{
    public static class LogHelper
    {
        public static string RequestPayload = "";

        public static async void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            diagnosticContext.Set("RequestBody", RequestPayload);

            string responseBodyPayload = await ReadResponseBody(httpContext.Response);
            diagnosticContext.Set("RequestMethod", request.Method);
            diagnosticContext.Set("ClientIp", value: httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString());
            diagnosticContext.Set("ResponseBody", responseBodyPayload);

            var responseBody = JsonConvert.DeserializeObject<ApiResponse>(responseBodyPayload);

            if (responseBody != null)
            {
                diagnosticContext.Set("ResponseCode", responseBody.StatusCode);
                diagnosticContext.Set("Description", responseBody.Message);
            }
            
            // Set all the common properties available for every request
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            var clientId = request.Headers["client_id"].ToString();

            diagnosticContext.Set("ClientId", clientId);

            var correlationId = request.Headers["x-correlation-id"].ToString() ?? Guid.NewGuid().ToString();

            diagnosticContext.Set("CorrelationId", correlationId);

            httpContext.Response.Headers.Append("x-correlation-id", correlationId);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{responseBody}";
        }
    }
}
