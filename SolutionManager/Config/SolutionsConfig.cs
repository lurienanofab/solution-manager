using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SolutionManager.Config;

public class SolutionsConfig
{
    [JsonProperty("msbuildPath")]
    public string MSBuildPath { get; set; }

    [JsonProperty("solutionBasePath")]
    public string SolutionBasePath { get; set; }

    [JsonProperty("tests", Required = Required.Always)]
    public IEnumerable<TestItem> Tests { get; set; }

    [JsonProperty("backups", Required = Required.Always)]
    public IEnumerable<BackupItem> Backups { get; set; }

    [JsonProperty("solutions")]
    public IEnumerable<SolutionItem> Solutions { get; set; }

    public static SolutionsConfig Load(string configPath)
    {
        if (File.Exists(configPath))
            return JsonConvert.DeserializeObject<SolutionsConfig>(File.ReadAllText(configPath));
        else
            throw new Exception($"File does not exist: {configPath}");
    }
}