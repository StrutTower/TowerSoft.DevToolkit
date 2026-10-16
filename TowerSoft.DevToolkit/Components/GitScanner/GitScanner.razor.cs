using LibGit2Sharp;
using Microsoft.AspNetCore.Components;
using System.IO;
using TowerSoft.DevToolkit.Models;
using TowerSoft.DevToolkit.Services;
using TowerSoft.DevToolkit.Utilities;

namespace TowerSoft.DevToolkit.Components.GitScanner {
    public partial class GitScanner {
        private string folder;
        private bool scanning = false;
        private string scanningProgress;
        private string scanningFolder;
        private bool error;
        private string errorMessage;
        private bool includeReposWithoutChanges = false;
        private List<GitRepoInfo> gitRepoInfos = [];

        [Inject]
        public AppSettingsService AppSettingsService { get; set; }

        protected override void OnInitialized() {
            //folder = @"%USERPROFILE%\source\repos";
            folder = AppSettingsService.AppSettings.GitScannerDefaultPath;
        }

        private async Task StartScan() {
            Progress<string> progressReporter = new Progress<string>();
            progressReporter.ProgressChanged += (prop, data) => {
                string[] parts = data.Split(["||"], StringSplitOptions.RemoveEmptyEntries);
                scanningProgress = parts[0];
                scanningFolder = parts[1];
                StateHasChanged();
            };
            //await Scan(progressReporter);


            await ScanNew(progressReporter);

            //ScanNew();
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
                progress.Report($"{currentRepoCount} / {directories.Length}||{dir}");

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
                string output = await ProcessUtilities.GetStandardOutputTest("git.exe", "status -sb", di.Parent.FullName);
                //if (!string.IsNullOrWhiteSpace(statusResult.Error)) {
                //    info.ErrorOccurred = true;
                //    info.ErrorMessage = statusResult.Error;
                //} else {
                if (output.Contains("ahead", StringComparison.OrdinalIgnoreCase)) {
                    info.LocalAheadOfRemote = true;
                }
                //}

                gitRepoInfos.Add(info);
                currentRepoCount++;
            }
            scanning = false;
            scanningProgress = null;
        }

        private async Task ScanNew(IProgress<string> progress) {
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
                progress.Report($"{currentRepoCount} / {directories.Length}||{dir}");
                
                DirectoryInfo di = new(dir);

                using Repository repo = new(dir);

                RepositoryStatus test = repo.RetrieveStatus(new StatusOptions());

                var info = new GitRepoInfo() {
                    RepoDirectory = di.Parent,
                    ScannedFolder = Environment.ExpandEnvironmentVariables(folder),
                    ErrorOccurred = false,
                    Changes = []
                };


                foreach(var item in test) {
                    string prefix = string.Empty;
                    switch (item.State) {
                        case FileStatus.NewInIndex:
                        case FileStatus.NewInWorkdir:
                            prefix = "AA ";
                            break;
                        case FileStatus.ModifiedInIndex:
                        case FileStatus.RenamedInIndex:
                        case FileStatus.TypeChangeInIndex:
                        case FileStatus.ModifiedInWorkdir:
                        case FileStatus.TypeChangeInWorkdir:
                        case FileStatus.RenamedInWorkdir:
                            prefix = "MM ";
                            break;
                        case FileStatus.DeletedFromIndex:
                        case FileStatus.DeletedFromWorkdir:
                            prefix = "DD ";
                            break;
                        //case FileStatus.Nonexistent:
                        //    break;
                        //case FileStatus.Unaltered:
                        //    break;
                            //break;
                        //case FileStatus.Unreadable:
                        //    break;
                        case FileStatus.Ignored:
                            continue;
                        //case FileStatus.Conflicted:
                        //    break;
                    }
                    info.Changes.Add(prefix + item.FilePath);
                }

                if (repo.Stashes.Count() > 0) {
                    info.Stashes = repo.Stashes.Select(x => x.Message).ToList();
                }

                if (repo.Head.TrackingDetails.AheadBy > 0)
                    info.LocalAheadOfRemote = true;

                gitRepoInfos.Add(info);
                currentRepoCount++;
            }
            scanning = false;
            scanningProgress = null;
        }
    }
}