using System.Net;
using System.Reflection;
using CoyposServer.Models;
using CoyposServer.Utils;
using CoyposServer.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;

namespace CoyposServer.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private string ApiKey => EnvVars.ServerApiKey;

    public ApiKeyMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (context?.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.MethodInfo?.GetCustomAttribute<NoApiKeyAttribute>() is not null)
        {
            Log.Msg($"🔓 {context.Connection.RemoteIpAddress} - no auth required.", "REST");
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("XApiKey", out var extractedApiKey) || extractedApiKey != ApiKey)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            Log.Wrn($"🔒 {context.Connection.RemoteIpAddress} failed to authenticate!", "REST");
            return;
        }
        Log.Msg($"🔓 {context.Connection.RemoteIpAddress} authenticated.", "REST");
        await _next(context);
    }
}