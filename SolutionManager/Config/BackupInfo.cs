using Newtonsoft.Json;

namespace SolutionManager.Config;

public class BackupInfo : ConfigInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }
}