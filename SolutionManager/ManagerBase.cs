using SolutionManager.Config;
using SolutionManager.Options;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static SimpleExec.Command;

namespace SolutionManager;

public abstract class ManagerBase
{
    public delegate void OutputHandler(object sender, OutputEventArgs e);

    public event OutputHandler Output;

    public SolutionsConfig Config { get; }

    public ManagerBase(SolutionsConfig config)
    {
        Config = config;
    }

    protected async Task<RunProcessResult> RunProcessAsync(string name, string args, bool noEcho = true)
    {
        int code = 0;

        var text = await ReadAsync(
            name: name,
            args: args,
            noEcho: noEcho,
            createNoWindow: true,
            handleExitCode: x =>
            {
                code = x;
                return true;
            }
        );

        var result = new RunProcessResult(code, text);

        return result;
    }

    protected virtual SolutionItem[] FilterSolutions(CommandLineOptions opts)
    {
        var solutions = string.IsNullOrEmpty(opts.SolutionName)
            ? Config.Solutions.ToArray()
            : Config.Solutions.Where(x => x.Solution.Name == opts.SolutionName).ToArray();
        return solutions;
    }

    protected virtual void WriteOutput(string text, bool isError = false)
    {
        if (isError)
            WriteOutput(new OutputEventArgs(text, Color.Red));
        else
            WriteOutput(new OutputEventArgs(text));
    }

    protected void WriteOutput(OutputEventArgs e)
    {
        Output?.Invoke(this, e);
    }
}