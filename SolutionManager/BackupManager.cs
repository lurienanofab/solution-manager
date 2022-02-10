using SolutionManager.Config;
using SolutionManager.Options;
using System;
using System.IO;
using System.Linq;

namespace SolutionManager;

public class BackupManager : ManagerBase
{
    public BackupManager(SolutionsConfig config) : base(config) { }

    public void Backup(BackupOptions opts)
    {
        var solutions = FilterSolutions(opts);

        foreach (var sol in solutions)
        {
            if (sol.Backup.Enabled)
            {
                WriteOutput("---------------------------------------------------------------------------");
                WriteOutput(sol.Solution.Name);
                WriteOutput("---------------------------------------------------------------------------");

                try
                {
                    var backup = Config.Backups.FirstOrDefault(x => x.Name == sol.Backup.Name);
                    
                    if (backup == null)
                        throw new Exception($"Cannot find backup: {sol.Backup.Name}");

                    var src = Path.Combine(backup.Source, sol.Solution.Name);
                    var dest = Path.Combine(backup.Destination, sol.Solution.Name);

                    if (!Directory.Exists(src))
                        throw new Exception($"Source directory does not exist: {src}");

                    if (Directory.Exists(dest))
                    {
                        Directory.Delete(dest, true);
                        WriteOutput($"deleted: {dest}");
                    }

                    DirectoryCopy(src, dest, true);
                }
                catch (Exception ex)
                {
                    WriteOutput(ex.Message, true);
                }
            }
        }
    }

    //https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
    private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        var dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.
        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
            WriteOutput(tempPath);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }
}