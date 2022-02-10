using SolutionManager.Config;
using SolutionManager.Options;
using System.IO;
using System.Threading.Tasks;

namespace SolutionManager;

public class PublishManager : ManagerBase
{
    public PublishManager(SolutionsConfig config) : base(config) { }

    public async Task PublishAsync(PublishOptions opts)
    {
        var solutions = FilterSolutions(opts);

        foreach (var sol in solutions)
        {
            if (sol.Publish.Enabled)
            {
                WriteOutput("---------------------------------------------------------------------------");
                WriteOutput($"{sol.Solution.Name} ({sol.Publish.Configuration}) | {sol.Publish.Profile}");
                WriteOutput("---------------------------------------------------------------------------");

                string name = Path.Combine(Config.MSBuildPath, "MSBuild.exe");
                string file = Path.Combine(Config.SolutionBasePath, sol.Solution.Path, $"{sol.Solution.Name}.sln");
                string args = $"{file} -noLogo /v:minimal /p:Configuration={sol.Publish.Configuration} /p:DeployOnBuild=true /p:PublishProfile=\"{sol.Publish.Profile}\"";

                var output = await RunProcessAsync(name, args);
                WriteOutput(output.Text, output.Code > 0);
            }
        }
    }
}