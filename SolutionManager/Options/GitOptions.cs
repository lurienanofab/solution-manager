using CommandLine;

namespace SolutionManager.Options;

[Verb("git", false, HelpText = "Execute git commands for solutions in config.json")]
public class GitOptions : CommandLineOptions
{
    [Option('a', "action", Required = false, Default = "status", HelpText = "The git action to perform (status, diff, etc)")]
    public string Action { get; set; }
}