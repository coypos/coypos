using System.Net;
using CoyposServer.Middleware;
using CoyposServer.Utils;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;
using System.Web;

namespace CoyposServer;

public class Setup
{
    public static void App()
    { 
        var builder = WebApplication.CreateBuilder();
                // Cors

        var origins = new string[]
        {
            "https://smilginp.evolpe.net",
            "http://localhost:8080",
            "http://localhost:5016",
            "https://localhost:7091"
        };
        Log.Msg($"Adding origins:", "Setup - CORS");
        foreach (var origin in origins)
            Log.Msg(origin, "Setup - CORS");
        builder.Services.AddCors(o => o.AddDefaultPolicy(policy => {
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
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
        
        
        var app = builder.Build();

        // Swagger app
        if (app.Environment.IsDevelopment())
        {
            Log.Msg("Enabling Swagger...", "Setup - Swagger");
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
        }

        app.UseHttpsRedirection();

        // <> UseCors must be placed after UseRouting and before UseAuthorization. This is to ensure that CORS headers are included in the response for both authorized and unauthorized calls.
        app.UseCors();
        // </>
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<ApiKeyMiddleware>();
        app.Use(async (context, next) =>
        {
            Log.Msg($"🔄 Performing {context.Connection.RemoteIpAddress}'s request...", "REST");
            await next.Invoke();
            Log.Msg($"✅ Sent '{((HttpStatusCode)context.Response.StatusCode).ToString()}' response to {context.Connection.RemoteIpAddress}", "REST");
        });
        app.UseAuthorization();
        app.MapControllers();

        Log.Msg($"✅ Done! Listening on port 80...", "Setup");
        app.Run();
    }
}