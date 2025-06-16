using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Jennifer.Api;

public class OpenApiSecuritySchemeTransformer
    : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info.Title = "Jennifer Account Management";
        document.Info.Description = "Jennifer Account Management API, Base on EF Core Identity";
        document.Info.Contact = new OpenApiContact
        {
            Name = "Jennifer API",
            Email = "h20913@gmail.com",
            Url = new Uri($"https://github.com/nameofSEOKWONHONG/Jennifer")
        };

        var securitySchema =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            };

        var securityRequirement =
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    []
                }
            };
        
        document.SecurityRequirements.Add(securityRequirement);
        document.Components = new OpenApiComponents()
        {
            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>()
            {
                { "Bearer", securitySchema }
            }
        };
        return Task.CompletedTask;
    }
}