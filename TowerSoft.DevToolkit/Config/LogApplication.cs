using System.ComponentModel.DataAnnotations;

namespace TowerSoft.DevToolkit.Config {
    public class LogApplication {
        public string Guid { get; set; }

        [Required]
        public string Name { get; set; }

        public List<LogInstance> Logs { get; set; }
    }
}
