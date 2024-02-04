using Checkout.PaymentGateway.Api.Authentication;
using Checkout.PaymentGateway.Api.RateLimits;
using Checkout.PaymentGateway.Application.Clients;
using Checkout.PaymentGateway.Application.Commands;
using Checkout.PaymentGateway.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("AcquirerClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7253");
});
builder.Services.AddMediatR(typeof(CreatePaymentCommand));
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IIdempotencyKeyRepository, IdempotencyKeyRepository>();
builder.Services.AddScoped<IAcquirerClient, AcquirerClient>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
const string fixedPolicy = "fixed";
var myOptions = new RateLimitOptions();
builder.Configuration.GetSection("RateLimitOptions").Bind(myOptions);


builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode =StatusCodes.Status429TooManyRequests;
    rateLimiterOptions.AddFixedWindowLimiter(fixedPolicy, options =>
    {
        options.Window = TimeSpan.FromSeconds(myOptions.Window);
        options.PermitLimit = myOptions.PermitLimit;
        options.QueueLimit = myOptions.QueueLimit;
        options.QueueProcessingOrder = myOptions.QueueProcessingOrder;
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey",new OpenApiSecurityScheme
    {
        Description = "The Checkout.com API key to use",
        Type = SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    
    var requirement = new OpenApiSecurityRequirement
    {
        { scheme, new List<string>() }
    };
    c.AddSecurityRequirement(requirement);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseRateLimiter();

app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
