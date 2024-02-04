using Checkout.PaymentGateway.Api.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Checkout.PaymentGateway.Api.Tests.Authentication
{
    [TestFixture]
    public class ApiKeyAuthMiddlewareTests
    {
        private Mock<RequestDelegate> _mockNext = new Mock<RequestDelegate>();
        private Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();
        private const string API_KEY_CONFIG_PATH = "Authentication:ApiKey";
        private const string VALID_API_KEY = "valid-api-key";

        [SetUp]
        public void SetUp()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Adjust this part to avoid using GetValue extension method
            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.SetupGet(m => m.Value).Returns(VALID_API_KEY);
            _mockConfiguration.Setup(c => c.GetSection(API_KEY_CONFIG_PATH)).Returns(configSectionMock.Object);
        }


        [Test]
        public async Task Invoke_ApiKeyMissing_ReturnsUnauthorized()
        {
            // Arrange
            var middleware = new ApiKeyAuthMiddleware(_mockNext.Object, _mockConfiguration.Object);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.Invoke(context);

            
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That((HttpStatusCode)context.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(responseBody, Is.EqualTo("Api Key missing"));
            });
        }

        [Test]
        public async Task Invoke_ApiKeyInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var middleware = new ApiKeyAuthMiddleware(_mockNext.Object, _mockConfiguration.Object);
            var context = new DefaultHttpContext
            {
                Response =
                {
                    Body = new MemoryStream()
                }
            };

            const string invalidApiKey = "invalid-api-key";
            context.Request.Headers[AuthConstants.API_KEY_HEADER_NAME] = invalidApiKey;

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            
            Assert.Multiple(() =>
            {
                Assert.That((HttpStatusCode)context.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(responseBody, Is.EqualTo("Invalid API key"));
            });
        }

        [Test]
        public async Task Invoke_ApiKeyValid_CallsNextDelegate()
        {
            // Arrange
            var middleware = new ApiKeyAuthMiddleware(_mockNext.Object, _mockConfiguration.Object);
            var context = new DefaultHttpContext();

            const string validApiKey = "valid-api-key";
            context.Request.Headers[AuthConstants.API_KEY_HEADER_NAME] = validApiKey;

            var wasNextDelegateCalled = false;
            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask).Callback(() => wasNextDelegateCalled = true);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.That(wasNextDelegateCalled, Is.True, "The next delegate in the middleware pipeline was not called.");
        }

    }
}
