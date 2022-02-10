using Newtonsoft.Json;

namespace SolutionManager.Config;

public class ConfigInfo
{
    [JsonProperty("enabled", Required = Required.Always)]
    public bool Enabled { get; set; }
}