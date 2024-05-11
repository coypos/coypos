using System.Diagnostics;
using System.Net;
using CoyposServer.Middleware;
using CoyposServer.Utils;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;
using System.Web;
using CoyposServer.BackgroundServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CoyposServer;

public class Setup
{
    public static void App()
    {
        WebApplication app;
        try
        {
            EnvVars.VerifyEnvVars();
            
            var builder = WebApplication.CreateBuilder();
            // Cors

            var origins = new string[]
            {
                "https://smilginp.evolpe.net",
                "https://adminsmilginp.evolpe.net",
                "http://localhost:8080",
                "http://localhost:5016",
                "https://localhost:7091"
            };
            Log.Msg($"Adding origins:", "Setup - CORS");
            foreach (var origin in origins)
                Log.Msg(origin, "Setup - CORS");
            builder.Services.AddCors(o => o.AddDefaultPolicy(policy =>
            {
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

            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                var ip = "";
                try
                {
                    ip = Dns.GetHostAddresses("db").First().ToString();
                }
                catch (Exception e)
                {
                    Log.Msg("Failed to resolve db");
                    return;
                }

                Log.Msg($"Resolved db as {ip}");
                
                options.UseSqlServer(
                    $"Server={ip},{EnvVars.DatabasePort};Database=master;User Id={EnvVars.DatabaseUser};Password={EnvVars.DatabasePass};encrypt=False");
            });
            
            builder.Services.AddSingleton<IModelBinderProvider, JsonModelBinderProvider>();

            builder.Services.AddMvc(options =>
            {
                options.ModelBinderProviders
                    .Insert(0, new JsonModelBinderProvider(
                        builder.Services.BuildServiceProvider().GetService<DatabaseContext>()));
            });

            builder.Services.AddHostedService<AsyncSetupService>();

            app = builder.Build();

            // Swagger app
            //if (app.Environment.IsDevelopment())
            //{
                Log.Msg("Enabling Swagger...", "Setup - Swagger");
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
            //}

            app.UseHttpsRedirection();

            // <> UseCors must be placed after UseRouting and before UseAuthorization. This is to ensure that CORS headers are included in the response for both authorized and unauthorized calls.
            app.UseCors();
            // </>
            app.UseRouting();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseMiddleware<BodyEnricherMiddleware>();
            
            app.Use(async (context, next) =>
            {
                Log.Msg($"🔄 Performing {context.Connection.RemoteIpAddress}'s request...", "REST");
                var timer = Stopwatch.StartNew();
                await next.Invoke();

                Log.Msg(
                $"✅ Sent '{((HttpStatusCode)context.Response.StatusCode).ToString()}' response to {context.Connection.RemoteIpAddress} ({(int)timer.Elapsed.TotalMilliseconds}ms)",
                "REST");
            });
            app.UseAuthorization();
            app.MapControllers();

            Log.Msg($"✅ Done! Listening on port 80...", "Setup");
        }
        catch (Exception e)
        {
            Log.Err("❌ Setup failure => " + e.Message, "Setup");
            if (e.StackTrace != null) Log.Err(e.StackTrace);
            return;
        }

        try
        {
            app.Run();
        }
        catch (Exception e)
        {
            Log.Err("❌ Runtime error => " + e.Message);
            if (e.StackTrace != null) Log.Err(e.StackTrace);
            return;
        }
    }
}
