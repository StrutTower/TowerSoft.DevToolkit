namespace TowerSoft.DevToolkit.Config {
    public class LogInstance {
        public string Guid { get; set; }

        public string Name { get; set; }

        public string LogFolder { get; set; }

        public string LogFilenamePattern { get; set; }

        public RollingType RollingType { get; set; }
    }
}
