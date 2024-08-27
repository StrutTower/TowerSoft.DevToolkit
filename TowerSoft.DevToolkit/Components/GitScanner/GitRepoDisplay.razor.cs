using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using TowerSoft.DevToolkit.Models;

namespace TowerSoft.DevToolkit.Components.GitScanner {
    public partial class GitRepoDisplay {
        [Parameter] public GitRepoInfo Model { get; set; }

        [Parameter] public bool IncludeReposWithoutChanges { get; set; }


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

        private string GetRelativePath(string path) {
            string newPath = path;

            if (newPath.StartsWith(Model.ScannedFolder, StringComparison.OrdinalIgnoreCase))
                newPath = newPath[Model.ScannedFolder.Length..];

            if (newPath.StartsWith('\\'))
                newPath = newPath[1..];

            return newPath;
        }
    }
}
