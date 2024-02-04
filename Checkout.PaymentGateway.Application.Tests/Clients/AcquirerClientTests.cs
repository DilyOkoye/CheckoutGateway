using System.Net;
using Checkout.PaymentGateway.Application.Clients;
using Checkout.PaymentGateway.Application.Clients.Request;
using Checkout.PaymentGateway.Application.Clients.Response;
using Checkout.PaymentGateway.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Checkout.PaymentGateway.Application.Tests.Clients;

[TestFixture]
public class AcquirerClientTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory = new Mock<IHttpClientFactory>();
    private Mock<ILogger<AcquirerClient>> _mockLogger = new Mock<ILogger<AcquirerClient>>();
    private AcquirerClient _acquirerClient;

    [SetUp]
    public void SetUp()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger<AcquirerClient>>();
        _acquirerClient = new AcquirerClient(_mockHttpClientFactory.Object, _mockLogger.Object);
    }

    private void SetupHttpClient(MockHttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://test.com/") };
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
    }

    private ClientRequest CreateValidClientRequest() => new ClientRequest
    {
        Currency = "GBP",
        Amount = 10,
        Cvv = "123",
        ExpirationDate = "01/2027",
        HolderName = "Dili Okoye",
        CardNumber = "5105105105105100"
    };

    [Test]
    public async Task AcquirerPayment_WhenSuccess_ReturnsClientResponse()
    {
        // Arrange
        var expectedResponse = new ClientResponse { TransactionStatus = TransactionStatus.Approved };
        var handler = new MockHttpMessageHandler(JsonConvert.SerializeObject(expectedResponse), HttpStatusCode.OK);
        SetupHttpClient(handler);

        // Act
        var result = await _acquirerClient.AcquirerPayment(CreateValidClientRequest());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.TransactionStatus, Is.EqualTo(TransactionStatus.Approved));
            Assert.That(handler.NumberOfCalls, Is.EqualTo(1), "The HTTP call should be made exactly once.");
        });
    }

    [Test]
    public async Task AcquirerPayment_ShouldHandleNonSuccessHttpResponse()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(JsonConvert.SerializeObject(new ClientResponse()), HttpStatusCode.BadGateway);
        SetupHttpClient(handler);

        // Act
        var result = await _acquirerClient.AcquirerPayment(CreateValidClientRequest());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.TransactionStatus, Is.EqualTo(TransactionStatus.InternalError));
        });

    }

    [Test]
    public async Task AcquirerPayment_ShouldHandleUnexpectedExceptionAndRetryOnTransientFailure()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(_ => throw new HttpRequestException("Network failure"));
        SetupHttpClient(handler);

        // Act
        var result = await _acquirerClient.AcquirerPayment(CreateValidClientRequest());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result?.TransactionStatus, Is.EqualTo(TransactionStatus.InternalError));

            // Considering Polly's default retry policy (assuming it retries 3 times for this test setup)
            Assert.That(handler.NumberOfCalls, Is.EqualTo(4), "Should include the initial call plus retries.");
        });
        
    }
}