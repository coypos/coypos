using System.Diagnostics;
using CoyposServer.Middleware;
using CoyposServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace CoyposServer;
class Program
{
    public static Stopwatch UptimeStopwatch;
    
    static void Main(string[] args)
    {
        UptimeStopwatch = Stopwatch.StartNew();
        Log.Msg("🔄 Server starting...");
        Setup.App();
        Log.Dispose();
    }
}