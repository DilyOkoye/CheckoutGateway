using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Application.Interfaces;
using Checkout.PaymentGateway.Domain.Responses;
using Checkout.PaymentGateway.Infrastructure.Repositories;

namespace Checkout.PaymentGateway.Application.Queries
{
    public class GetPaymentQueryHandler(ITransactionRepository transactionRepository)
        : IQueryHandler<GetPaymentQuery, GetPaymentResponse?>
    {
        public async Task<GetPaymentResponse?> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
        {
           var response = await transactionRepository.GetTransactionAsync(request.Id);
           if (response != null)
           {
               return new GetPaymentResponse()
               {
                   Card = new Card()
                   {
                       ExpirationDate = response.Card.ExpirationDate,
                       Number = "**** **** **** " + response.Card.Number[^4..],
                       HolderName = response.Card.HolderName
                   },
                   TransactionStatus = response.TransactionStatus.ToString(),
                   Merchant = response.Merchant,
                   Currency = response.Currency,
                   Amount = response.Amount * 100,
                   PaymentId = response.PaymentId
               };
           }

           return null;
        }
    }
}
