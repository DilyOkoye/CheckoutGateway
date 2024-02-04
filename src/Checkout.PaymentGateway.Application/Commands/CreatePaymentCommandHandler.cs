using Checkout.PaymentGateway.Application.Clients;
using Checkout.PaymentGateway.Application.Clients.Response;
using Checkout.PaymentGateway.Application.Mapper;
using Checkout.PaymentGateway.Application.Validators;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Enums;
using Checkout.PaymentGateway.Domain.Exceptions;
using Checkout.PaymentGateway.Domain.Responses;
using Checkout.PaymentGateway.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Application.Commands;

public class CreatePaymentCommandHandler(
    ITransactionRepository transactionRepository,
    IIdempotencyKeyRepository idempotencyKeyRepository,
    IAcquirerClient acquirerClient,
    ILogger<CreatePaymentCommandHandler> logger)
    : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
{
    public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check for idempotency
            var existingPayment = await CheckForIdempotencyAsync(request.IdempotencyKeyId);
            if (existingPayment != null)
                return existingPayment;

            // Validate and process payment
            var validatedPayment = ValidatePaymentCommand.ValidatePayment(request);
            var paymentEntity = PaymentMapper.CommandToEntity(validatedPayment);
            await transactionRepository.CreateTransactionAsync(paymentEntity);

            // Acquirer logic
            var acquirerResponse = await ProcessPaymentWithAcquirerAsync(paymentEntity);
            paymentEntity.TransactionStatus = acquirerResponse.TransactionStatus;

            // Finalize payment
            await FinalizePaymentAsync(request, paymentEntity);

            return new CreatePaymentResponse
            {
                PaymentReference = paymentEntity.PaymentId,
                TransactionCode = acquirerResponse.TransactionStatus.ToString(),
            };
        }
        catch (Exception ex) when (ex is not PaymentValidationException and not DuplicateTransactionException)
        {
            logger.LogError(ex, "Unhandled exception in CreatePayment.");
            throw new CreatePaymentCommandException("An error occurred while processing the payment.", ex);
        }
    }

    private async Task<CreatePaymentResponse?> CheckForIdempotencyAsync(Guid idempotencyKeyId)
    {
        var idempotencyKey = await idempotencyKeyRepository.GetIdempotencyKeyAsync(idempotencyKeyId);
        if (idempotencyKey != null)
        {
            return new CreatePaymentResponse
            {
                PaymentReference = idempotencyKey.PaymentId,
                TransactionCode = TransactionStatus.Approved.ToString(),
            };
        }
        return null;
    }

    private async Task<ClientResponse> ProcessPaymentWithAcquirerAsync(Payment payment)
    {
        var acquirerRequest = payment.PaymentToAcquirerRequest();
        var acquirerResponse = await acquirerClient.AcquirerPayment(acquirerRequest);
        return acquirerResponse ?? throw new Exception("Failed to process payment with acquirer.");
    }

    private async Task FinalizePaymentAsync(CreatePaymentCommand command, Payment payment)
    {
        if (payment.TransactionStatus == TransactionStatus.Approved)
        {
            var idempotency = PaymentMapper.MapToIdempotencyKey(command.IdempotencyKeyId, payment.PaymentId);
            await idempotencyKeyRepository.CreateTransactionAsync(idempotency);
        }
        await transactionRepository.UpdateTransactionAsync(payment);
    }
}