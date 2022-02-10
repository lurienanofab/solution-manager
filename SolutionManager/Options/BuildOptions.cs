using CommandLine;
using System;

namespace SolutionManager.Options;

[Verb("build", false, HelpText = "Build solutions in config.json")]
public class BuildOptions : CommandLineOptions
{
    [Option('c', "config", Required = false, Default = "Debug", HelpText = "The build configuration, for example Debug or Release")]
    public string ConfigurationName { get; set; }
    public Action<string> Log { get; set; }
}