using System.Diagnostics;
using System.Net;
using CoyposServer.Models;
using CoyposServer.Utils;
using Microsoft.AspNetCore.Mvc;
using NickStrupat;

namespace CoyposServer.Controllers;

public class CoyposController : ControllerBase
{
    private DatabaseContext _dbContext;
    
    public CoyposController(DatabaseContext dbContext) =>
        _dbContext = dbContext;
    
    /// <summary>
    /// Returns system & application info
    /// </summary>
    [HttpGet]
    [Route("coypos")]
    public ObjectResult Coypos()
    {
        var computerInfo = new ComputerInfo();
        var coypos = new CoyposModel()
        {
            Version = "0.4", //todo
            Time = DateTime.Now,
            MemoryUsed = (ulong)Process.GetCurrentProcess().PrivateMemorySize64,
            MemoryFree = (computerInfo.AvailablePhysicalMemory),
            MemoryTotal = computerInfo.TotalPhysicalMemory,
            OsName = computerInfo.OSFullName,
            OsPlatform = computerInfo.OSPlatform,
            OsVersion = computerInfo.OSVersion,
            DockerContainerId = System.IO.File.ReadAllText("/etc/hostname").ReplaceLineEndings(""),
            Uptime = Program.UptimeStopwatch.Elapsed
        };
        return StatusCode((int)HttpStatusCode.OK, coypos);
    }

    [HttpGet]
    [Route("logs")]
    public ObjectResult Logs()
    {
        return StatusCode((int)HttpStatusCode.OK, Log.LocalCache);
    }

    [HttpGet]
    [Route("clear_images")]
    public async Task<ObjectResult> ClearImages()
    {
        var deleted = await ClearUnknownImages();
        return StatusCode((int)HttpStatusCode.OK, $"Deleted {deleted} unlinked images from the database.");
    }
    
    private async Task<int> ClearUnknownImages()
    {
        var counter = 0;
        foreach (var image in _dbContext.Images)
            if (_dbContext.Products.FirstOrDefault(_ => _.Image == image.ID.ToString()) is null)
            {
                _dbContext.Images.Remove(image);
            }

        if (counter > 0)
            await _dbContext.SaveChangesAsync();
        return counter;
    }
}