using CoyposServer.Middleware;
using CoyposServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;

namespace CoyposServer;
class Program
{
    static void Main(string[] args)
    {
        Log.Msg("🔄 Server starting...");
        Setup.App();
    }
    
}