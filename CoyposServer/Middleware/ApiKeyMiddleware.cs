using System.Net;
using CoyposServer.Models;
using CoyposServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoyposServer.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKey = "samoa-rostra-biometry"; //todo: move me to config

    public ApiKeyMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext context)
    {
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