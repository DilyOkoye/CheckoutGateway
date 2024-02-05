using System.Net;

namespace Checkout.PaymentGateway.Application.Tests.Clients
{
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;
        private readonly HttpStatusCode _statusCode;
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseSelector;

        public int NumberOfCalls { get; private set; } = 0;

        public MockHttpMessageHandler(string responseContent, HttpStatusCode statusCode)
        {
            _responseContent = responseContent;
            _statusCode = statusCode;
        }

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseSelector)
        {
            _responseSelector = responseSelector;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NumberOfCalls++;

            var responseMessage = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent)
            };

            return Task.FromResult(responseMessage);
        }
    }
}
