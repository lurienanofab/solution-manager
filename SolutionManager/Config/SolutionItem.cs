using Newtonsoft.Json;

namespace SolutionManager.Config;

public class SolutionItem
{
    [JsonProperty("solution", Required = Required.Always)]
    public SolutionInfo Solution { get; set; }

    [JsonProperty("git", Required = Required.Always)]
    public ConfigInfo Git { get; set; }

    [JsonProperty("build", Required = Required.Always)]
    public ConfigInfo Build { get; set; }

    [JsonProperty("backup", Required = Required.Always)]
    public BackupInfo Backup { get; set; }

    [JsonProperty("publish", Required = Required.Always)]
    public PublishInfo Publish { get; set; }

    [JsonProperty("test", Required = Required.Always)]
    public TestInfo Test { get; set; }
}