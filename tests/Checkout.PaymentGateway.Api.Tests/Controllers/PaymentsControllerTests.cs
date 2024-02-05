using Checkout.PaymentGateway.Api.Controllers;
using Checkout.PaymentGateway.Application.Commands;
using Checkout.PaymentGateway.Application.Queries;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Enums;
using Checkout.PaymentGateway.Domain.Exceptions;
using Checkout.PaymentGateway.Domain.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Checkout.PaymentGateway.Api.Tests.Controllers;

[TestFixture]
public class PaymentsControllerTests
{
    private Mock<IMediator> _mockMediator = new Mock<IMediator>();
    private Mock<ILogger<PaymentsController>> _mockLogger = new Mock<ILogger<PaymentsController>>();
    private PaymentsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<PaymentsController>>();
        _controller = new PaymentsController(_mockMediator.Object, _mockLogger.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Test]
    public async Task CreatePayment_WithValidCommand_ReturnsOk()
    {
        // Arrange
        var idempotencyKey = Guid.NewGuid().ToString();
        _controller.Request.Headers["Idempotency-Key"] = idempotencyKey;

        var command = new CreatePaymentCommand();
        var response = new CreatePaymentResponse
        {
            PaymentReference = Guid.NewGuid(),
            TransactionCode = TransactionStatus.Approved.ToString()
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreatePaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreatePayment(command) as OkObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            _mockMediator.Verify(x => x.Send(It.Is<CreatePaymentCommand>(c => c.IdempotencyKeyId == Guid.Parse(idempotencyKey)), It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Test]
    public async Task GetPaymentById_WithValidId_ReturnsOk()
    {
        // Arrange
        var paymentId = Guid.NewGuid().ToString();
        var getPaymentResponse = new GetPaymentResponse()
        {
            Currency = "GBP",
            Card = new Card()
            {
                ExpirationDate = "01/2027",
                Cvv = "322",
                HolderName = "Dili Okoye",
                Number = "5105105105105100"
            },
            Amount = 10,
            Merchant = new Merchant()
            {
                Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
            }
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetPaymentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getPaymentResponse);

        // Act
        var result = await _controller.GetPaymentById(paymentId) as OkObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            _mockMediator.Verify(x => x.Send(It.Is<GetPaymentQuery>(q => q.Id == Guid.Parse(paymentId)), It.IsAny<CancellationToken>()), Times.Once);
        });
    }


    [Test]
    public async Task CreatePayment_WithInvalidIdempotencyKey_ReturnsBadRequest()
    {
        // Arrange
        _controller.Request.Headers["Idempotency-Key"] = "invalid-guid";

        var command = new CreatePaymentCommand()
        {
            Currency = "GBP",
            Card = new Card()
            {
                ExpirationDate = "01/2027",
                Cvv = "322",
                HolderName = "Dili Okoye",
                Number = "5105105105105100"
            },
            Amount = 10,
            IdempotencyKeyId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
            Merchant = new Merchant()
            {
                Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
            }
        };

        // Act
        var result = await _controller.CreatePayment(command) as BadRequestObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        });
    }


    [Test]
    public async Task CreatePayment_WithModelStateValidationFailure_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("ErrorKey", "ErrorMessage");
        var command = new CreatePaymentCommand();

        // Act
        var result = await _controller.CreatePayment(command) as BadRequestObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result?.Value, Is.InstanceOf<SerializableError>(), "Should return validation errors.");
        });
    }


    [Test]
    public async Task CreatePayment_WhenPaymentValidationExceptionIsThrown_ReturnsBadRequestWithErrorMessage()
    {
        // Arrange
        var idempotencyKey = Guid.NewGuid().ToString();
        _controller.Request.Headers["Idempotency-Key"] = idempotencyKey;

        var command = new CreatePaymentCommand()
        {
            Currency = "GBP",
            Card = new Card()
            {
                ExpirationDate = "01/2027",
                Cvv = "322",
                HolderName = "Dili Okoye",
                Number = "5105105105100"
            },
            Amount = 10,
            IdempotencyKeyId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
            Merchant = new Merchant()
            {
                Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
            }
        };


        const string validationExceptionMessage = "Invalid payment details.";

        _mockMediator.Setup(x => x.Send(It.IsAny<CreatePaymentCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PaymentValidationException(validationExceptionMessage));

        // Act
        var result = await _controller.CreatePayment(command) as BadRequestObjectResult;
        var errorResponse = result?.Value as dynamic;
        var transactionStatusProperty = errorResponse?.GetType().GetProperty("TransactionStatus");
        var validationMessage = transactionStatusProperty?.GetValue(errorResponse, null).ToString();


        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.AreEqual(validationExceptionMessage, validationMessage);
        });

    }
}