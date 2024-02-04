using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Checkout.PaymentGateway.Infrastructure.Tests.Repositories;

[TestFixture]
public class TransactionRepositoryTests
{
    private IMemoryCache _cache;
    private TransactionRepository _repository;

    [SetUp]
    public void Setup()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _repository = new TransactionRepository(_cache);
    }

    [TearDown]
    public void TearDown()
    {
        _cache.Dispose();
    }

    [Test]
    public async Task GetTransactionAsync_ReturnsTransaction_IfExists()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var expectedPayment = new Payment { PaymentId = paymentId };
        _cache.Set(paymentId, expectedPayment);

        // Act
        var result = await _repository.GetTransactionAsync(paymentId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPayment));
    }

    [Test]
    public async Task CreateTransactionAsync_ReturnsPayment_IfNotExists()
    {
        // Arrange
        var payment = new Payment { PaymentId = Guid.NewGuid() };

        // Act
        var result = await _repository.CreateTransactionAsync(payment);

        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result, Is.EqualTo(payment));
            Assert.That(_cache.TryGetValue(payment.PaymentId, out Payment? cachedPayment), Is.True);
            Assert.That(cachedPayment, Is.EqualTo(payment));
        });
    }

    [Test]
    public async Task UpdateTransactionAsync_ReturnsUpdatedPayment_IfExists()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var originalPayment = new Payment { PaymentId = paymentId, Amount = 100 };
        _cache.Set(paymentId, originalPayment);

        var updatedPayment = new Payment { PaymentId = paymentId, Amount = 200 };

        // Act
        var result = await _repository.UpdateTransactionAsync(updatedPayment);

        // Assert
        Assert.That(result.Amount, Is.EqualTo(updatedPayment.Amount));
    }
}