using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

[ApiController]
[Route("ktofajnyjest")]
public class KtoFajnyJestController : ControllerBase
{
    [HttpGet(Name = "ktofajnyjest")]
    public string Get() => "Kajetan";
}