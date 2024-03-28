using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using SimpleIdempotency.Filters;

namespace SimpleIdempotency.Swagger
{
    public sealed class AddIdempotencyKeyHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = IdempotencyHeaders.IdempotencyKey,
                In = ParameterLocation.Header,
                Description = "Idempotency Key",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}