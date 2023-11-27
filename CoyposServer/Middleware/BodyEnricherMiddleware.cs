using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoyposServer.Middleware;

/// <summary>
/// An enricher middleware that acts as a workaround for Axios not supporting
/// GET requests with a body. When the request has a 'body' argument with an
/// escaped JSON to be parsed, it will replace the request body on fly, making ASPNET
/// think that it received a body when in reality it didn't. :)
/// </summary>
public class BodyEnricherMiddleware
{
    private readonly RequestDelegate _next;

    public BodyEnricherMiddleware(RequestDelegate next, ILogger<BodyEnricherMiddleware> logger)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Find the [FromBody] argument
        var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

        var hasBodyArgument = context.Request.Query.ContainsKey("body");
        var hasBody = context.Request.ContentLength.HasValue && context.Request.ContentLength.Value > 0;
        var requiresBody = actionDescriptor is not null && (actionDescriptor.MethodInfo.GetCustomAttributes()
            .Any(a => a.GetType() == typeof(HttpGetAttribute)));

        if (!hasBodyArgument && !hasBody && requiresBody)
        {
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(""));
            context.Request.ContentType = "application/json";
        }
        
        if (hasBodyArgument && !hasBody && requiresBody)
        {
            var parameters = actionDescriptor?.MethodInfo.GetParameters();
            ParameterInfo? foundParameter = null;
            if (parameters is not null)
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.CustomAttributes.FirstOrDefault(a =>
                            a.AttributeType == typeof(FromBodyAttribute)) is not null)
                    {
                        foundParameter = parameter;
                        break;
                    }
                }
            }

            // If the [FromBody] argument has been found...
            if (foundParameter is not null)
            {
                var bodyFromRequest = new MemoryStream(Encoding.UTF8.GetBytes(context.Request.Query["body"]!));
                context.Request.Body = bodyFromRequest;
                context.Request.ContentType = "application/json";
            }
        }

        // Call the next middleware in the pipeline
        
        //var bod = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync();

        await _next(context);
    }

    public void EnrichFromSql()
    {
        
    }
}