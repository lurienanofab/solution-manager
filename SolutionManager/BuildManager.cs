using SolutionManager.Config;
using SolutionManager.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SolutionManager;

public class BuildManager : ManagerBase
{
    public BuildManager(SolutionsConfig config) : base(config) { }

    public async Task BuildAsync(BuildOptions opts)
    {
        try
        {
            var solutions = FilterSolutions(opts);

            foreach (var sol in solutions)
            {
                if (sol.Build.Enabled)
                {
                    WriteOutput("---------------------------------------------------------------------------");
                    WriteOutput($"{sol.Solution.Name} ({opts.ConfigurationName})");
                    WriteOutput("---------------------------------------------------------------------------");

                    string name = Path.Combine(Config.MSBuildPath, "MSBuild.exe");
                    string file = Path.Combine(Config.SolutionBasePath, sol.Solution.Path, $"{sol.Solution.Name}.sln");
                    string args = $"{file} -noLogo /v:minimal /p:Configuration={opts.ConfigurationName}";

                    var output = await RunProcessAsync(name, args);
                    WriteOutput(output.Text, output.Code > 0);
                }
            }
        }
        catch (Exception ex)
        {
            WriteOutput(ex.Message, true);
        }
    }
}