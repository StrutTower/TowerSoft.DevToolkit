using System.IO;
using TowerSoft.DevToolkit.Models;
using TowerSoft.DevToolkit.Utilities;

namespace TowerSoft.DevToolkit.Components.GitScanner {
    public partial class GitScanner {
        private string folder;
        private bool scanning = false;
        private string scanningProgress;
        private bool error;
        private string errorMessage;
        private bool includeReposWithoutChanges = false;
        private List<GitRepoInfo> gitRepoInfos = [];

        protected override void OnInitialized() {
            folder = @"%USERPROFILE%\source\repos";
        }

        private async Task StartScan() {
            Progress<string> progressReporter = new Progress<string>();
            progressReporter.ProgressChanged += (prop, data) => {
                scanningProgress = data;
                StateHasChanged();
            };
            await Scan(progressReporter);
        }

        private async Task Scan(IProgress<string> progress) {
            scanning = true;
            error = false;
            errorMessage = null;
            gitRepoInfos.Clear();

            string path = Environment.ExpandEnvironmentVariables(folder);
            if (!Directory.Exists(path)) {
                error = true;
                errorMessage = $"Unable to find path '{path}'";
                scanning = false;
                return;
            }
            string[] directories = null;

            await Task.Run(() => {
                directories = Directory.GetFileSystemEntries(path, ".git", SearchOption.AllDirectories);
            });

            int currentRepoCount = 1;

            foreach (string dir in directories) {
                progress.Report($"{currentRepoCount} / {directories.Length}");

                if (dir.Contains("node_modules")) continue;
                DirectoryInfo di = new(dir);

                // Check for changes
                ProcessResult changesResult = await ProcessUtilities.GetOutput("git.exe", "status --porcelain", di.Parent.FullName);
                if (!string.IsNullOrWhiteSpace(changesResult.Error)) {
                    gitRepoInfos.Add(new() {
                        RepoDirectory = di.Parent,
                        ScannedFolder = Environment.ExpandEnvironmentVariables(folder),
                        ErrorOccurred = true,
                        ErrorMessage = changesResult.Error
                    });
                    continue;
                }

                GitRepoInfo info = new() {
                    RepoDirectory = di.Parent,
                    ScannedFolder = Environment.ExpandEnvironmentVariables(folder),
                    ErrorOccurred = false,
                    Changes = [.. changesResult.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries)]
                };

                // Check for stashes
                ProcessResult stashResult = await ProcessUtilities.GetOutput("git.exe", "stash list", di.Parent.FullName);
                if (!string.IsNullOrWhiteSpace(stashResult.Error)) {
                    info.ErrorOccurred = true;
                    info.ErrorMessage = stashResult.Error;
                } else {
                    info.Stashes = [.. stashResult.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries)];
                }

                // Check if repo is ahead of remote
                ProcessResult statusResult = await ProcessUtilities.GetOutput("git.exe", "status -sb", di.Parent.FullName);
                if (!string.IsNullOrWhiteSpace(statusResult.Error)) {
                    info.ErrorOccurred = true;
                    info.ErrorMessage = statusResult.Error;
                } else {
                    if (statusResult.Output.Contains("ahead", StringComparison.OrdinalIgnoreCase)) {
                        info.LocalAheadOfRemote = true;
                    }
                }

                gitRepoInfos.Add(info);
                currentRepoCount++;
            }
            scanning = false;
            scanningProgress = null;
        }
    }
}