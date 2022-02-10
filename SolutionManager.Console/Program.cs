using CommandLine;
using SolutionManager.Config;
using SolutionManager.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace SolutionManager.Console;

class Program
{
    static SolutionsConfig _config;

    static string _solution = string.Empty;
    static bool _pause = false;

    static void GetConfig()
    {
        var configPath = ConfigurationManager.AppSettings["ConfigPath"];

        if (string.IsNullOrEmpty(configPath))
            throw new Exception($"The required setting ConfigPath is missing in App.config appSettings.");

        var f = new FileInfo(configPath);

        if (!f.Exists)
            throw new Exception($"Cannot find configuration file: {configPath}. Make sure the correct path is specified in App.config appSettings.");

        _config = SolutionsConfig.Load(f.FullName);
    }

    static async Task Main(string[] args)
    {
        GetConfig();

        if (args.Length > 0)
        {
            await Parser.Default.ParseArguments<BuildOptions, PublishOptions, BackupOptions, GitOptions, TestOptions>(args)
                .MapResult(
                    (BuildOptions opts) => RunBuildAndReturnExitCode(opts),
                    (PublishOptions opts) => RunPublishAndReturnExitCode(opts),
                    (BackupOptions opts) => RunBackupAndReturnExitCode(opts),
                    (GitOptions opts) => RunGitAndReturnExitCode(opts),
                    (TestOptions opts) => RunTestAndReturnExitCode(opts),
                    errs => HandleParseErrors(errs));
            Pause();
        }
        else
        {
            while (true)
            {
                System.Console.WriteLine("command:");
                var cmd = System.Console.ReadLine();

                if (cmd == "exit")
                {
                    break;
                }
                else
                {
                    System.Console.WriteLine($"solution [{_solution}]:");
                    var temp = System.Console.ReadLine();
                    if (!string.IsNullOrEmpty(temp))
                        _solution = temp;
                    if (_solution == "all")
                        _solution = string.Empty;

                    if (cmd == "build")
                    {
                        System.Console.WriteLine("config [Debug]:");
                        var config = System.Console.ReadLine();
                        if (string.IsNullOrEmpty(config)) config = "Debug";

                        await RunBuildAndReturnExitCode(new BuildOptions { SolutionName = _solution, ConfigurationName = config });
                    }
                    else if (cmd == "publish")
                    {
                        await RunPublishAndReturnExitCode(new PublishOptions { SolutionName = _solution });
                    }
                    else if (cmd == "backup")
                    {
                        await RunBackupAndReturnExitCode(new BackupOptions { SolutionName = _solution });
                    }
                    else if (cmd == "git")
                    {
                        System.Console.WriteLine("action [status]:");
                        var action = System.Console.ReadLine();
                        if (string.IsNullOrEmpty(action)) action = "status";

                        await RunGitAndReturnExitCode(new GitOptions { SolutionName = _solution, Action = action });
                    }
                    else if (cmd == "test")
                    {
                        await RunTestAndReturnExitCode(new TestOptions { SolutionName = _solution });
                    }
                }
            }
        }
    }

    static async Task<int> RunBuildAndReturnExitCode(BuildOptions opts)
    {
        _pause = opts.Pause;
        BuildManager builder = new(_config);
        builder.Output += HandleOutput;
        await builder.BuildAsync(opts);
        return 0;
    }

    static async Task<int> RunPublishAndReturnExitCode(PublishOptions opts)
    {
        _pause = opts.Pause;
        PublishManager publish = new(_config);
        publish.Output += HandleOutput;
        await publish.PublishAsync(opts);
        return 0;
    }

    static async Task<int> RunBackupAndReturnExitCode(BackupOptions opts)
    {
        _pause = opts.Pause;
        var backup = new BackupManager(_config);
        backup.Output += HandleOutput;
        backup.Backup(opts);
        return await Task.FromResult(0);
    }

    static async Task<int> RunGitAndReturnExitCode(GitOptions opts)
    {
        _pause = opts.Pause;
        var git = new GitManager(_config);
        git.Output += HandleOutput;
        await git.ExecuteAsync(opts);
        return 0;
    }

    static async Task<int> RunTestAndReturnExitCode(TestOptions opts)
    {
        _pause = opts.Pause;
        var test = new TestManager(_config);
        test.Output += HandleOutput;
        await test.TestAsync(opts);
        return 0;
    }

    static async Task<int> HandleParseErrors(IEnumerable<Error> _)
    {
        return await Task.FromResult(1);
    }

    static void HandleOutput(object sender, OutputEventArgs e)
    {
        var fclr = System.Console.ForegroundColor.FromConsoleColor();
        var bclr = System.Console.BackgroundColor.FromConsoleColor();

        for (var i = 0; i < e.Text.Length; i++)
        {
            System.Console.ForegroundColor = e.GetForeColorAtPosition(i, fclr).ToConsoleColor();
            System.Console.BackgroundColor = e.GetBackColorAtPosition(i, bclr).ToConsoleColor();
            System.Console.Write(e.Text[i]);
        }

        System.Console.Write("\n");

        System.Console.ForegroundColor = fclr.ToConsoleColor();
        System.Console.BackgroundColor = bclr.ToConsoleColor();
    }

    static void Pause()
    {
        if (_pause)
        {
            System.Console.Write("Press any key to exit...");
            System.Console.ReadKey(true);
        }
    }
}