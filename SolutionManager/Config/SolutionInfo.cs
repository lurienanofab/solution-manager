using Newtonsoft.Json;

namespace SolutionManager.Config;

public class SolutionInfo
{
    private string _path;

    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty("path")]
    public string Path
    {
        get => string.IsNullOrEmpty(_path) ? Name.ToLower() : _path;
        set => _path = value;
    }
}