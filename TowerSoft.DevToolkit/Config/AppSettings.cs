using System.Security.Cryptography.X509Certificates;

namespace TowerSoft.DevToolkit.Config {
    public class AppSettings {
        public string GitScannerDefaultPath { get; set; }

        public List<LogApplication> LogApplications { get; set; }
    }
}
