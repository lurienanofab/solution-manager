using Newtonsoft.Json;

namespace SolutionManager.Config;

public class TestInfo : ConfigInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }
}