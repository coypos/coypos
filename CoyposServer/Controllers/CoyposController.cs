using System.Diagnostics;
using System.Net;
using CoyposServer.Models;
using Microsoft.AspNetCore.Mvc;
using NickStrupat;

namespace CoyposServer.Controllers;

public class CoyposController : ControllerBase
{
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
            Version = "0.1", //todo
            Time = DateTime.Now,
            MemoryUsed = (ulong)Process.GetCurrentProcess().PrivateMemorySize64,
            MemoryFree = (computerInfo.AvailablePhysicalMemory),
            MemoryTotal = computerInfo.TotalPhysicalMemory,
            OsName = computerInfo.OSFullName,
            OsPlatform = computerInfo.OSPlatform,
            OsVersion = computerInfo.OSVersion,
            DockerContainerId = System.IO.File.ReadAllText("/etc/hostname").ReplaceLineEndings("")
        };
        return StatusCode((int)HttpStatusCode.OK, coypos);
    }
}