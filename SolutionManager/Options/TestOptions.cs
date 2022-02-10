using CommandLine;

namespace SolutionManager.Options;

[Verb("test", false, HelpText = "Test solutions in config.json")]
public class TestOptions : CommandLineOptions { }