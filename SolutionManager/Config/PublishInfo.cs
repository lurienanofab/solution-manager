using Newtonsoft.Json;

namespace SolutionManager.Config;

public class PublishInfo : ConfigInfo
{
    [JsonProperty("profile")]
    public string Profile { get; set; }

    [JsonProperty("configuration")]
    public string Configuration { get; set; }
}