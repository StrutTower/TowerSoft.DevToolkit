using LibGit2Sharp;
using Microsoft.AspNetCore.Components;
using System.IO;
using TowerSoft.DevToolkit.Models;
using TowerSoft.DevToolkit.Services;

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
            folder = AppSettingsService.AppSettings.GitScannerDefaultPath;
        }

        private async Task StartScan() {
            Progress<string> progressReporter = new();
            progressReporter.ProgressChanged += (prop, data) => {
                string[] parts = data.Split(["||"], StringSplitOptions.RemoveEmptyEntries);

                scanningProgress = parts[0];
                if (parts.Length > 1)
                    scanningFolder = parts[1];
                StateHasChanged();
            };

            scanningFolder = null;
            scanningProgress = null;

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
                progress.Report($"{currentRepoCount} / {directories.Length}||{dir}");

                DirectoryInfo di = new(dir);

                using Repository repo = new(dir);

                RepositoryStatus test = repo.RetrieveStatus(new StatusOptions());

                GitRepoInfo info = new GitRepoInfo() {
                    RepoDirectory = di.Parent,
                    ScannedFolder = Environment.ExpandEnvironmentVariables(folder),
                    ErrorOccurred = false,
                    Changes = []
                };


                foreach (var item in test) {
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