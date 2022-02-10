using SolutionManager.Config;
using SolutionManager.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SolutionManager;

public class GitManager : ManagerBase
{
    private GitOptions _options;

    public GitManager(SolutionsConfig config) : base(config) { }

    public async Task ExecuteAsync(GitOptions opts)
    {
        _options = opts;

        var solutions = FilterSolutions(_options);

        foreach (var sol in solutions)
        {
            if (sol.Git.Enabled)
            {
                WriteOutput("---------------------------------------------------------------------------");
                WriteOutput(sol.Solution.Name);
                WriteOutput("---------------------------------------------------------------------------");
                await GitCommand(Path.Combine(Config.SolutionBasePath, sol.Solution.Path, ".git"), _options.Action);
            }
        }
    }

    private async Task GitCommand(string path, string command)
    {
        //using (var repo = new LibGit2Sharp.Repository(@"g:\dev\github\jgett\testrepo\.git"))
        using (var repo = new LibGit2Sharp.Repository(path))
        {
            if (command == "status")
            {
                var status = await Task.FromResult(repo.RetrieveStatus(new LibGit2Sharp.StatusOptions()));

                var blank = new OutputEventArgs(string.Empty);

                var staged = new List<StatusInfo>();
                var unstaged = new List<StatusInfo>();
                var untracked = new List<StatusInfo>();

                /***** staged ************************/
                foreach (var x in status.Removed)
                { 
                    staged.Add(new StatusInfo("deleted", x.FilePath));
                }

                foreach (var x in status.Staged)
                {
                    staged.Add(new StatusInfo("modified", x.FilePath));
                }

                foreach (var x in status.Added)
                {
                    staged.Add(new StatusInfo("new file", x.FilePath));
                }

                /***** unstaged **********************/
                foreach (var x in status.Modified)
                    unstaged.Add(new StatusInfo("modified", x.FilePath));

                foreach (var x in status.Missing)
                    unstaged.Add(new StatusInfo("deleted", x.FilePath));

                /***** untracked *********************/
                foreach (var x in status.Untracked)
                    untracked.Add(new StatusInfo(null, x.FilePath));

                StatusInfo[] ordered;

                if (staged.Count > 0)
                {
                    WriteOutput(new OutputEventArgs("Changes to be committed:"));
                    WriteOutput(blank);

                    ordered = staged.OrdinalSort(x => x.File);
                    foreach (var info in ordered)
                    {
                        WriteOutput(new OutputEventArgs(info.ToString(), Color.Green));
                    }

                    //foreach (var x in staged["deleted"])
                    //    WriteOutput(new OutputEventArgs($"        deleted:    {x}", ConsoleColor.Green, null));

                    //foreach (var x in staged["modified"])
                    //    WriteOutput(new OutputEventArgs($"        modified:   {x}", ConsoleColor.Green, null));

                    //foreach (var x in staged["renamed"])
                    //    WriteOutput(new OutputEventArgs($"        renamed:    {x}", ConsoleColor.Green, null));

                    //foreach (var x in staged["new file"])
                    //    WriteOutput(new OutputEventArgs($"        new file:   {x}", ConsoleColor.Green, null));

                    WriteOutput(blank);
                }

                if (unstaged.Count > 0)
                {
                    WriteOutput(new OutputEventArgs("Changes not staged for commit:"));
                    WriteOutput(blank);

                    ordered = unstaged.OrdinalSort(x => x.File);
                    foreach (var info in ordered)
                    {
                        WriteOutput(new OutputEventArgs(info.ToString(), Color.Red));
                    }

                    //foreach (var x in unstaged["modified"])
                    //    WriteOutput(new OutputEventArgs($"        modified:   {x}", ConsoleColor.Red, null));

                    //foreach (var x in unstaged["deleted"])
                    //    WriteOutput(new OutputEventArgs($"        deleted:    {x}", ConsoleColor.Red, null));

                    WriteOutput(blank);
                }

                if (untracked.Count > 0)
                {
                    WriteOutput(new OutputEventArgs("Untracked files:"));
                    WriteOutput(blank);

                    ordered = untracked.OrdinalSort(x => x.File);
                    foreach (var info in ordered)
                    {
                        WriteOutput(new OutputEventArgs(info.ToString(), Color.Red));
                    }

                    //foreach (var x in untracked)
                    //    WriteOutput(new OutputEventArgs($"        {x}", ConsoleColor.Red, null));

                    WriteOutput(blank);
                }

                if (staged.Count + unstaged.Count + untracked.Count == 0)
                {
                    WriteOutput(new OutputEventArgs("nothing to commit, working tree clean"));
                    WriteOutput(blank);
                }
            }
            else
            {
                throw new NotImplementedException(command);
            }
        }
    }

    protected override void WriteOutput(string text, bool isError = false)
    {
        if (isError)
        {
            base.WriteOutput(text, isError);
            return;
        }

        var lines = text.Split(Convert.ToChar("\n"));

        if (lines.Length == 1)
        {
            base.WriteOutput(text, false);
        }
        else
        {
            var diffStart = false;
            var statusStart = false;

            foreach (var line in lines)
            {
                switch (_options.Action)
                {
                    case "status":
                        if (line.StartsWith("Changes not staged for commit:"))
                        {
                            statusStart = true;
                            WriteOutput(line, false);
                        }
                        else if (line.StartsWith("Untracked files:"))
                        {
                            statusStart = true;
                            WriteOutput(line, false);
                        }
                        else
                        {
                            if (statusStart)
                            {
                                if (line.StartsWith("\t"))
                                {
                                    WriteOutput(new OutputEventArgs(line, Color.Red));
                                }
                                else if (line.StartsWith("  ") || string.IsNullOrEmpty(line))
                                {
                                    WriteOutput(line, false);
                                }
                                else
                                {
                                    statusStart = false;
                                    WriteOutput(line, false);
                                }
                            }
                            else
                            {
                                WriteOutput(line, false);
                            }
                        }
                        break;
                    case "diff":
                        var args = new OutputEventArgs(line);

                        var m = Regex.Match(line, "^(@@\\s.+\\s@@) .+");

                        if (m.Groups.Count > 1)
                        {
                            diffStart = true;
                            args.AddFormat(m.Groups[1].Index, m.Groups[1].Length, Color.Cyan, null);
                            WriteOutput(args);
                        }
                        else
                        {
                            if (diffStart)
                            {
                                if (line.StartsWith("+"))
                                    WriteOutput(new OutputEventArgs(line, Color.Green));
                                else if (line.StartsWith("-"))
                                    WriteOutput(new OutputEventArgs(line, Color.Red));
                                else
                                    WriteOutput(args);
                            }
                            else
                            {
                                WriteOutput(args);
                            }
                        }
                        break;
                    default:
                        base.WriteOutput(line, false);
                        break;
                }
            }
        }
    }
}

public struct StatusInfo
{
    public StatusInfo(string status, string file)
    {
        Status = status;
        File = file;
    }

    public string Status { get; }
    public string File { get; }

    public override string ToString()
    {
        string result;

        if (string.IsNullOrEmpty(Status + File))
            return string.Empty;

        if (string.IsNullOrEmpty(Status))
        {
            result = File.PadLeft(8 + File.Length);
        }
        else
        {
            result = Status.PadLeft(8 + Status.Length) + ":";
            result = result.PadRight(20);
            result += File;
        }

        return result;
    }
}