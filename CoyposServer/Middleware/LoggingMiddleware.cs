using CoyposServer.Utils;

namespace CoyposServer.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        Log.Msg($"📨 Incoming {context.Request.Method} request to \"{context.Request.Path}\" from {context.Connection.RemoteIpAddress}...", "REST");
        await _next(context);
    }
}