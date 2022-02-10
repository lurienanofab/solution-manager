using CommandLine;

namespace SolutionManager.Options;

[Verb("backup", false, HelpText = "Backup solutions in config.json")]
public class BackupOptions : CommandLineOptions { }