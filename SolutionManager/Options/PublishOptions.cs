using CommandLine;

namespace SolutionManager.Options;

[Verb("publish", false, HelpText = "Publish solutions in config.json")]
public class PublishOptions : CommandLineOptions { }