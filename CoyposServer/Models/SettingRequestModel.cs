using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class SettingRequestModel
{
    [JsonProperty("key")] public string Key;
    [JsonProperty("value")] public string Value;
}