using CommandLine;

namespace SolutionManager.Options;

public abstract class CommandLineOptions
{
    [Option('s', "solution", Required = false, HelpText = "When provided, performs the action only on this solution.")]
    public string SolutionName { get; set; }

    [Option('p', "pause", Required = false, Default = false, HelpText = "When provided, execution will pause until any key is pressed (command line only).")]
    public bool Pause { get; set; }
}