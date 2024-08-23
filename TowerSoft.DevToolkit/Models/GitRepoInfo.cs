using System.IO;

namespace TowerSoft.DevToolkit.Models {
    public class GitRepoInfo {
        public DirectoryInfo RepoDirectory { get; set; }

        public List<string> Changes { get; set; }

        public List<string> Stashes { get; set; }

        public bool LocalAheadOfOrigin { get; set; }

        public bool ErrorOccurred { get; set; }

        public string ErrorMessage { get; set; }
    }
}
