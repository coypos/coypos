using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class ThingModel : ThingRequestModel
{
    /// <summary>
    /// Self-generated unique Thing ID
    /// </summary>
    [JsonProperty("id")] public int Id;
    
    /// <summary>
    /// Requested string but in reverse
    /// </summary>
    [JsonProperty("sampleGeneratedString")] public string SampleGeneratedString;
    
    /// <summary>
    /// Length of the requested string
    /// </summary>
    [JsonProperty("sampleGeneratedInt")] public int SampleGeneratedInteger;
}