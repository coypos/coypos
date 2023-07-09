using System.Net;
using CoyposServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private static List<ThingModel> _things = new();
    
    /// <summary>
    /// Returns all things
    /// </summary>
    [HttpGet]
    [Route("things")]
    [ProducesResponseType(typeof(List<ThingModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult Get()
    {
        try
        {
            return StatusCode((int)HttpStatusCode.OK, _things);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }
    
    /// <summary>
    /// Returns a specific thing
    /// </summary>
    /// <param name="thingId">Thing ID</param>
    [HttpGet]
    [Route("thing/{thingId:int}")]
    [ProducesResponseType(typeof(ThingModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetSpecific(int thingId)
    {
        try
        {
            // Exists?
            if (!_things.Exists(t => t.Id == thingId))
                return StatusCode((int)HttpStatusCode.BadRequest,
                    new ProblemDetails() { Title = $"No thing exists with ID of {thingId}." });
            
            return StatusCode((int)HttpStatusCode.OK, _things.First(t => t.Id == thingId));
        }
        catch (Exception e)
        {
            // Handle exceptions
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }

    /// <summary>
    /// Creates a new thing
    /// </summary>
    [HttpPost]
    [Route("thing")]
    [ProducesResponseType(typeof(ThingModel), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult Post([FromBody] ThingRequestModel requestThing)
    {
        try
        {
            // Parsed?
            if (string.IsNullOrEmpty(requestThing.SampleRequestString))
                return StatusCode((int)HttpStatusCode.BadRequest,
                    new ProblemDetails() { Title = "Invalid request body" });

            // Create...
            var thingToAdd = new ThingModel()
            {
                Id = _things.Count > 0 ? _things.MaxBy(t => t.Id).Id + 1 : 0,
                SampleRequestString = requestThing.SampleRequestString,
                SampleGeneratedInteger = requestThing.SampleRequestString.Length,
                SampleGeneratedString = new string(requestThing.SampleRequestString.Reverse().ToArray())
            };
            _things.Add(thingToAdd);
            
            // Output created entry
            return StatusCode((int)HttpStatusCode.Created, thingToAdd);
        }
        catch (Exception e)
        {
            // Handle exceptions
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }
    
    /// <summary>
    /// Deletes a specific thing
    /// </summary>
    /// <param name="thingId">Thing ID</param>
    [HttpDelete]
    [Route("thing/{thingId:int}")]
    [ProducesResponseType(typeof(ResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult Delete(int thingId)
    {
        try
        {
            // Exists?
            if (!_things.Exists(t => t.Id == thingId))
                return StatusCode((int)HttpStatusCode.BadRequest,
                    new ProblemDetails() { Title = $"No thing exists with ID of {thingId}." });
            
            // Delete
            _things.Remove(_things.First(t => t.Id == thingId));
            return StatusCode((int)HttpStatusCode.OK, new ResponseModel() { Info = $"Thing (ID {thingId}) deleted." });
        }
        catch (Exception e)
        {
            // Handle exceptions
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }
}
