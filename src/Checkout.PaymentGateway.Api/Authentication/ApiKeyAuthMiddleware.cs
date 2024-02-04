using System.Net;

namespace Checkout.PaymentGateway.Api.Authentication
{
    public class ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(AuthConstants.API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Api Key missing");
                return;
            }

            var apiKey = configuration.GetValue<string>(AuthConstants.API_KEY_SECTION_NAME);
            if (!apiKey!.Equals(extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Invalid API key");
                return;
            }
            
            await next(context);
        }
    }
}
