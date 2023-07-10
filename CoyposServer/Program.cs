using CoyposServer.Middleware;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

{
    // Cors
    builder.Services.AddCors(o => o.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(
                    "https://smilginp.evolpe.net",
                    "http://localhost:8080",
                    "http://localhost:5016",
                    "https://localhost:7091"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        }));

    // Controllers
    builder.Services.AddControllers();

    // Swagger builder
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddControllers().AddNewtonsoftJson();
    builder.Services.AddSwaggerGen(o =>
    {
        o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CoyposServer.xml"));
        o.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "XApiKey",
            In = ParameterLocation.Header
        });
        o.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    Scheme = "Bearer",
                    Name = "XApiKey",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });
    builder.Services.AddSwaggerGenNewtonsoftSupport();

}

var app = builder.Build();

{
    // Swagger app
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
    }

    app.UseHttpsRedirection();

// <> UseCors must be placed after UseRouting and before UseAuthorization. This is to ensure that CORS headers are included in the response for both authorized and unauthorized calls.
    app.UseCors();
// </>
    app.UseMiddleware<ApiKeyMiddleware>();
    app.UseAuthorization();
    app.MapControllers();
}

app.Run();
