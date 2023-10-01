using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class CoyposModel
{
    [JsonProperty("version")] public string Version { get; set; }
    [JsonProperty("time")] public DateTime Time { get; set; }
    [JsonProperty("memory_used")] public ulong MemoryUsed { get; set; }
    [JsonProperty("memory_free")] public ulong MemoryFree { get; set; }
    [JsonProperty("memory_total")] public ulong MemoryTotal { get; set; }
    [JsonProperty("os_platform")] public string OsPlatform { get; set; }
    [JsonProperty("os_version")] public string OsVersion { get; set; }
    [JsonProperty("os_name")] public string OsName { get; set; }
    [JsonProperty("docker_container_id")] public string DockerContainerId { get; set; }
}