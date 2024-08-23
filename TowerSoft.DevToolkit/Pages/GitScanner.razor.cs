using System.Diagnostics;
using System.IO;
using TowerSoft.DevToolkit.Models;
using TowerSoft.DevToolkit.Utilities;

namespace TowerSoft.DevToolkit.Pages {
    public partial class GitScanner {
        private string folder;
        private bool scanning = false;
        private bool error;
        private string errorMessage;
        private bool includeReposWithoutChanges = false;
        private List<GitRepoInfo> gitRepoInfos = [];

        protected override void OnInitialized() {
            folder = @"%USERPROFILE%\source\repos";
        }

        private async Task Scan() {
            scanning = true;
            gitRepoInfos.Clear();
            string path = Environment.ExpandEnvironmentVariables(folder);
            if (!Directory.Exists(path)) {
                error = true;
                errorMessage = $"Unable to find path '{path}'";
            }

            string[] directories = Directory.GetFileSystemEntries(path, ".git", SearchOption.AllDirectories);

            foreach (string dir in directories) {
                if (dir.Contains("node_modules")) continue;
                DirectoryInfo di = new(dir);

                // Check for changes
                ProcessResult changesResult = await ProcessUtilities.GetOutput("git.exe", "status --porcelain", di.Parent.FullName);
                if (!string.IsNullOrWhiteSpace(changesResult.Error)) {
                    gitRepoInfos.Add(new() {
                        RepoDirectory = di.Parent,
                        ErrorOccurred = true,
                        ErrorMessage = changesResult.Error
                    });
                    continue;
                }

                GitRepoInfo info = new() {
                    RepoDirectory = di.Parent,
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
                        info.LocalAheadOfOrigin = true;
                    }
                }

                gitRepoInfos.Add(info);
            }
            scanning = false;
        }

        private static void OpenFolder(string folderPath) {
            using Process process = new() {
                StartInfo = new ProcessStartInfo {
                    FileName = folderPath,
                    UseShellExecute = true,
                    Verb = "open"
                }
            };
            process.Start();
        }

        private static string ProcessChange(string change) {
            string prefix = change[..2];
            string file = change[3..];

            if (prefix == "??" || prefix.Contains('A')) {
                return " + " + file;
            }
            if (prefix.Contains('M')) {
                return " * " + file;
            }
            if (prefix.Contains('D')) {
                return " - " + file;
            }
            return prefix + " " + file;
        }

        private static string GetChangeClass(string change) {
            string prefix = change[..2];

            if (prefix == "??" || prefix.Contains('A')) {
                return "git-added";
            }
            if (prefix.Contains('M')) {
                return "git-modified";
            }
            if (prefix.Contains('D')) {
                return "git-removed";
            }

            return string.Empty;
        }
    }
}
