using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class ThingRequestModel
{
    /// <summary>
    /// Request string
    /// </summary>
    [JsonProperty("sampleRequestString")] public string SampleRequestString;
}