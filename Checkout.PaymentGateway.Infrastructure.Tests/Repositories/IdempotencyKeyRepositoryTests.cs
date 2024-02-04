using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Checkout.PaymentGateway.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class IdempotencyKeyRepositoryTests
    {
        private IMemoryCache _cache;
        private IdempotencyKeyRepository _repository;

        [SetUp]
        public void Setup()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _repository = new IdempotencyKeyRepository(_cache);
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
            var keyId = Guid.NewGuid();
            var expectedPayment = new IdempotencyKey(keyId, Guid.NewGuid());
            _cache.Set(keyId, expectedPayment);

            // Act
            var result = await _repository.GetIdempotencyKeyAsync(keyId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPayment));
        }

        [Test]
        public async Task CreateTransactionAsync_SetsIdempotencyKeyInCache()
        {
            // Arrange
            var idempotencyKey = new IdempotencyKey(Guid.NewGuid(), Guid.NewGuid());

            // Act
            var result = await _repository.CreateTransactionAsync(idempotencyKey);

            Assert.Multiple(() =>
            {

                // Assert
                Assert.That(result, Is.EqualTo(idempotencyKey));
                Assert.That(_cache.TryGetValue(idempotencyKey.Id, out IdempotencyKey? cachedPayment), Is.True);
                Assert.That(cachedPayment, Is.EqualTo(idempotencyKey));
            });
        }
    }
}
