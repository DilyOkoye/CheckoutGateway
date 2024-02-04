using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PaymentGateway.IdentityServer
{
    /// <summary>
    /// Create login api for swagger UI
    /// </summary>
    class AuthTokenOperation : IDocumentFilter
    {
        /// <summary>
        /// Add swagger end point to login
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) => swaggerDoc.Paths.Add("/auth/token", new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
    {
     {
      OperationType.Post,
      new OpenApiOperation
      {
       Tags = new List<OpenApiTag>
       {
        new OpenApiTag
        {
         Name="Auth"
        }
       },
       Summary = "Generate access token",
       RequestBody = new OpenApiRequestBody
       {
        Description = "Form data request body",
        Content = new Dictionary<string, OpenApiMediaType>
        {
         { "application/x-www-form-urlencoded",
          new OpenApiMediaType
          {
           Schema = new OpenApiSchema
           {
            Type="object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
             //{
             //    "grant_type",
             //    new OpenApiSchema
             //    {
             //         Description ="grant_type should be 'password'",
             //         Type = "string",
             //         Required = new HashSet<string> { "true" }
             //    }
             //},
             {
              "username",
              new OpenApiSchema
              {
             Description ="",
             Type = "string",
             Required = new HashSet<string> { "true" },
             Nullable = false
              }
             },
             {
              "password",
              new OpenApiSchema
              {
             Description ="",
             Type = "string",
             Format = "password",
             Required = new HashSet<string> { "true" },
             Nullable = false
              }
             }
            }
           }
          }
         }
        }
       },
       Responses = new OpenApiResponses
       {
        { "200",
       new OpenApiResponse
       {
         Description = "Returns generated authentication token with access claims and expiry time",
         Content = new Dictionary<string, OpenApiMediaType>
         {
          {
           "application/json",
           new OpenApiMediaType
           {
            Schema = new OpenApiSchema
            {
             Type="object",
             Properties = new Dictionary<string, OpenApiSchema>
             {
              {
               "access_token",
                new OpenApiSchema
                {
               Description ="Returns generated authentication token",
               Type = "string"
                }
              },
              {
               "expires_in",
                new OpenApiSchema
                {
               Description ="Returns expires time of the generated authentication token",
               Type = "string"
                }
              },
              {
               "token_type",
                new OpenApiSchema
                {
               Description ="Returns type of the token",
               Type = "string"
                }
              }
             }
            }
           }
          }
         }
       }
        },
        { "400",
       new OpenApiResponse
       {
         Description = "Returns invalid request details",
         Content = new Dictionary<string, OpenApiMediaType>
         {
          {
           "application/json",
           new OpenApiMediaType
           {
            Schema = new OpenApiSchema
            {
             Type="object",
             Properties = new Dictionary<string, OpenApiSchema>
             {
              {
               "error",
                new OpenApiSchema
                {
               Description ="Returns generated authentication token",
               Type = "string"
                }
              },
              {
               "error_message",
                new OpenApiSchema
                {
               Description ="Returns expires time of the generated authentication token",
               Type = "string"
                }
              }
             }
            }
           }
          }
         }
       }
        },
        { "500",
       new OpenApiResponse
       {
         Description = "Returns internal server error details",
         Content = new Dictionary<string, OpenApiMediaType>
         {
          {
           "application/json",
           new OpenApiMediaType
           {
            Schema = new OpenApiSchema
            {
             Type="object",
             Properties = new Dictionary<string, OpenApiSchema>
             {
              {
               "error",
                new OpenApiSchema
                {
               Description ="Returns generated authentication token",
               Type = "string"
                }
              },
              {
               "error_message",
                new OpenApiSchema
                {
               Description ="Returns expires time of the generated authentication token",
               Type = "string"
                }
              }
             }
            }
           }
          }
         }
       }
        }
       }
      }
     }
    }
        });
    }
}
