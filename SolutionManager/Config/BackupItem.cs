using Newtonsoft.Json;

namespace SolutionManager.Config;

public class BackupItem
{
    [JsonProperty("name", Required=Required.Always)]
    public string Name { get; set; }

    [JsonProperty("source", Required = Required.Always)]
    public string Source { get; set; }

    [JsonProperty("destination", Required = Required.Always)]
    public string Destination { get; set; }
}