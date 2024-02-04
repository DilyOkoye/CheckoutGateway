using System.Text;
using Checkout.PaymentGateway.Application.Clients.Request;
using Checkout.PaymentGateway.Application.Clients.Response;
using Checkout.PaymentGateway.Domain.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace Checkout.PaymentGateway.Application.Clients
{
    public class AcquirerClient(IHttpClientFactory httpClientFactory, ILogger<AcquirerClient> logger)
        : IAcquirerClient
    {
        public async Task<ClientResponse?> AcquirerPayment(ClientRequest clientRequest)
        {
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4)
                }, (exception, timeSpan, retryCount, _) =>
                {
                    // Log retries if needed
                    logger.LogWarning(
                        $"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to {exception.Message}");
                });

            try
            {
                return await retryPolicy.ExecuteAsync(async () =>
                {
                    var client = httpClientFactory.CreateClient("AcquirerClient");

                    var requestObject = JsonConvert.SerializeObject(clientRequest);

                    var response = await client.PostAsync("AcquiringBank/process-payment",
                        new StringContent(requestObject, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ClientResponse>(
                            await response.Content.ReadAsStringAsync());
                    }

                    // Handle non-successful HTTP response (e.g., log or throw an exception)
                    logger.LogError($"HTTP request failed with status code: {response.StatusCode}");
                    return new ClientResponse
                    {
                        TransactionStatus = TransactionStatus.InternalError,
                        TransactionMessage = $"HTTP request failed with status code: {response.StatusCode}",
                        // You can add more details or customize the response as needed.
                    };
                });
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                logger.LogError($"An unexpected error occurred: {ex.Message}");
                return new ClientResponse
                {
                    TransactionStatus = TransactionStatus.InternalError,
                    TransactionMessage = $"An unexpected error occurred",
                };
            }
        }
    }
}
