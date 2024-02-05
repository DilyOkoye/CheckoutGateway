# Checkout.PaymentGateway API
###### Responsible for validating requests, storing card information and forwarding payment requests and accepting payment responses to and from the acquiring bank.

General Flow Diagram
![alt text](https://www.websequencediagrams.com/cgi-bin/cdraw?lz=SWRlbXBvdGVuY3kKCkNsaWVudC0-U2VydmVyOiBQb3N0L1Bvc3RQYXltZW50e2kAJAota2V5Onh4eHh9Cm5vdGUgcmlnaHQgb2YgADUIUmVxdWVzdCBpcyBub24tADIJdChQcm9jZXNzIHRoZSBwAFQGKQoKAG8GLT4AgH8GOiA0MDAgKE5vIABpCyBrZXkAHwtEYXRhYmFzZToyMDA6IACBHQcgQ29uZmlybWVkCgCBAhcAgVgGIGhhcyBhbHJlYWR5IHNlZW4AgQoFAF4PLFxuAIE8DACBOgosXG4gRG8gbm90IHAAgUcHAIFDBwBvB292ZXIgAIFDCFJldHJ5AIIJCACCLjkAghERMjAwICgAgWgPYXRpb24pCg&s=default)


#### ✨Deliverables ✨
1. Build an API that allows a merchant:
a. To process a payment through your payment gateway.
b. To retrieve details of a previously made payment.
2. Build a bank simulator to test your payment gateway API.

#### ✨Assumptions ✨
- MerchantId and PaymentReference is known and passed from the merchant
- Acquiring Bank Simulator returns failure(bad request) for certain blacklisted cards, The blacklisted cards set up are : 5555555555554444,6011000990139424,3530111333300000
- Transaction Statuses are Approved, Declined, ProcessingError, SuspectedFraud, InternalError, Processing.

#### Software Design Approach
[Domain Driven Design](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/best-practice-an-introduction-to-domain-driven-design/) 
Mediator Design Pattern in C#

#### Implementation Strategy
ProcessPayment Controller that houses:-
- PostPayment
- RetrievePayment

#### Packages
- mediator implementation in .NET with query-command segregation; fits the functional paradigm quite nicely
- Swashbuckle(Comes directly in .Net 8)
- Microsoft.Extensions.Caching.Abstraction as a form of storage on memory
- Moq for mocking test 
- NUnit for test
- Polly is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Rate-limiting, Hedging and Fallback in a fluent and thread-safe manner.
-  Newtonsoft.Json

#### Running the Application
- .NET 8
- Docker where applicable
The application can be ran via:-
> Setting the Checkout.PaymentGateway.Api & AcquiringBank.Simulator as the startup project and hit f5 to launch 
- Swagger doc included as a documentation guide
- Sample request(s) located in the Checkout.PaymentGateway.Api/Assets

#### Project Structure
- Checkout.PaymentGateway.Api
- Checkout.PaymentGateway.Application
- Checkout.PaymentGateway.Domain
- Checkout.PaymentGateway.Infrastructure
- Checkout.PaymentGateway.Api.Tests
- Checkout.PaymentGateway.Application.Tests
- Checkout.PaymentGateway.Infrastructure.Tests
- AcquiringBank.Simulator

#### Checkout.PaymentGateway.Api
This project is the entry point api of the solution for handling the two important request
- POST/Payment PostPayment
- GET/Payment/RetrievePayment?id=10b2f23e-3d47-4281-a2ae-17163ce0402a

**Sample request for PostPayment**
```json
{
  "merchant": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  },
  "card": {
    "number": "378282246310005",
    "expirationDate": "01/2027",
    "cvv": "322",
    "holderName": "Dilichukwu Okoye"
  },
  "amount": 12,
  "currency": "GBP"
}
```

**Sample Successful Response**
```json
{
    "paymentRef": "b9511a1e-00d1-4617-a5ff-58c237b26c35",
    "transactionStatus": "Approved"
}
```

**GET/Payment/{id}**
Retrieves the previous transaction in the memory cache

**Sample Response**
```json
{
    "paymentId": "5aae1d58-9195-47aa-a7ed-8944c934408f",
    "merchant": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    },
    "card": {
        "number": "**** **** **** 0005",
        "expirationDate": "01/2027",
        "cvv": null,
        "holderName": "Dilichukwu Okoye"
    },
    "amount": 12.00,
    "currency": "GBP",
    "transactionStatus": "Approved"
}
```

**Error**: response status is 404 for payment reference not found

**Checkout.PaymentGateway.Application**
This class library contains the commandHandler implementation, Queries, Validators, Mappers, Acquirer Client 

**Checkout.PaymentGateway.Domain**
This is where the concepts of the business domain are. This layer has all the information about the business case and the business rules. Here’s where the entities are. 

**Checkout.PaymentGateway.Infrastructure**
This class library contains the repositories; interfaces and implementation of the saved request via MemoryCache

**Checkout.PaymentGateway.Api.Tests**
This project covers the unit tests for the two endpoints in the Payment Controller and ApiKeyAuthMiddleware tests

**Checkout.PaymentGateway.Application.Tests**
This project test the different scenarios for the commandHandler implementation, Queries, Validators, Mappers, Acquirer Client

**Checkout.PaymentGateway.Infrastructure.Tests**
This project covers the Repository Test via MemoryCache

**AcquiringBank.Simulator**
This class library is a responsible for simulating the responses from a bank.
It returns a successful response except for scenarios such as using a blacklisted card or a random responses that are not approved.

#### ✨Extra  Miles✨ 
**Security: Authentication**
Authentication via api key. An API key is an identifier assigned to an API client, used to authenticate an application calling the API. It is typically a unique alphanumeric string included in the API call, which the API receives and validates.

**Idempotency**
An idempotent endpoint is one that can be called any number of times while guaranteeing that the side effects will occur only once.
Idempotency is applied in the application by adding the Idempotency-Key in the api header for every request, when a post payment api request is sent without an idempotency-key the api responds with a bad request, however
a successful request is inserted into the idempotency database/table, On retrying a connection failure, on the second request the server will see the ID for the first time, and process it normally. The flow diagram below highlights the system architecture.
![alt text](https://www.websequencediagrams.com/cgi-bin/cdraw?lz=SWRlbXBvdGVuY3kKCkNsaWVudC0-U2VydmVyOiBQb3N0L1Bvc3RQYXltZW50e2kAJAota2V5Onh4eHh9Cm5vdGUgcmlnaHQgb2YgADUIUmVxdWVzdCBpcyBub24tADIJdChQcm9jZXNzIHRoZSBwAFQGKQoKAG8GLT4AgH8GOiA0MDAgKE5vIABpCyBrZXkAHwtEYXRhYmFzZToyMDA6IACBHQcgQ29uZmlybWVkCgCBAhcAgVgGIGhhcyBhbHJlYWR5IHNlZW4AgQoFAF4PLFxuAIE8DACBOgosXG4gRG8gbm90IHAAgUcHAIFDBwBvB292ZXIgAIFDCFJldHJ5AIIJCACCLjkAghERMjAwICgAgWgPYXRpb24pCg&s=default)

**Polly**
Polly is an open source framework for that "allows express transient exception and fault handling policies such as Retry, Retry Forever, Wait and Retry, or Circuit Breaker in a fluent manner". 
Retry policy using Polly to handle exceptions that might occur during an HTTP request in the AcquirerPayment method to the AcquiringBank.Simulator api.

**Rate Limiting**
Rate limiting is a technique used to restrict the number of requests or transactions that a client (e.g., an application or user) can make to a payment system within a specific time frame.
The rate limit configuration is set up in the appsettings.json in Checkout.PaymentGateway.Api project


**Areas of Improvement**
- A proper database infrastructure would be ideal instead of an in-memory state
- Complete containerization using docker
- Leveraging on Transactional outbox pattern and a proper pub/sub & queueing system (SNS,SQS) in place where once the payment or transaction is saved successfully in a database and a subsequently published to a queue, it can be subscribed via another instance to complete the transaction process
- A much more robust test case alongside with an automated unit test, typical scenarios such as an automated load test to ensure that the api can handle and respond to a specific threashold of request.
- Efficient file logging that can be fed to various data monitoring tools like Datadog
