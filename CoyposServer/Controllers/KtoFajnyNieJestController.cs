using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;


[ApiController]
[Route("ktofajnyniejest")]
public class KtoFajnyNieJestController : ControllerBase
{
    [HttpGet(Name = "ktofajnyniejest")]
    public string Get() => "Tomasz";
}