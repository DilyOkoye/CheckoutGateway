using Checkout.PaymentGateway.Application.Commands;
using Checkout.PaymentGateway.Application.Queries;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Enums;
using Checkout.PaymentGateway.Domain.Exceptions;
using Checkout.PaymentGateway.Domain.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Checkout.PaymentGateway.Api.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting("fixed")]
public class PaymentsController(IMediator mediator, ILogger<PaymentsController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private const string IDEMPOTENCY_KEY_HEADER = "Idempotency-Key";
    private readonly ILogger<PaymentsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private bool TryParseIdempotencyKey(out Guid key)
    {
        key = Guid.Empty;
        if (!Request.Headers.TryGetValue(IDEMPOTENCY_KEY_HEADER, out var idempotencyKey))
        {
            return false;
        }

        var keyValue = idempotencyKey.FirstOrDefault();
        return Guid.TryParse(keyValue, out key);
    }

    [HttpPost("PostPayment")]
    [ProducesResponseType(typeof(CreatePaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePayment(CreatePaymentCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryParseIdempotencyKey(out var idempotencyId))
        {
            return BadRequest($"{IDEMPOTENCY_KEY_HEADER} header is not valid.");
        }

        command.IdempotencyKeyId = idempotencyId;

        try
        {
            var result = await _mediator.Send(command);
            if (result.TransactionCode == TransactionStatus.Approved.ToString())
            {
                return Ok(new { paymentRef = (Guid?)result.PaymentReference, TransactionStatus = result.TransactionCode });
            }
            return BadRequest(new { paymentRef = result?.PaymentReference, TransactionStatus = result?.TransactionCode });
        }
        catch (PaymentValidationException ex)
        {
            _logger.LogError(ex, "Payment validation exception");
            return BadRequest(new { TransactionStatus = ex.Message});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in CreatePayment.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("RetrievePayment")]
    [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaymentById(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var paymentId))
        {
            return BadRequest("Transaction Id is not valid.");
        }

        try
        {
            var query = new GetPaymentQuery(paymentId);
            var result = await _mediator.Send(query);

            return result == null ? NotFound(new { paymentRef = id }) : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unhandled error in GetPaymentById.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unhandled error occurred." });
        }
    }
}