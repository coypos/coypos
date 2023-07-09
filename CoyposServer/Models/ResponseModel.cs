using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class ResponseModel
{
    /// <summary>
    /// Additional response info
    /// </summary>
    [JsonProperty("info", NullValueHandling = NullValueHandling.Ignore)]
    public string? Info;
}