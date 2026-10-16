using System.ComponentModel.DataAnnotations;

namespace TowerSoft.DevToolkit.Config {
    public enum RollingType {
        [Display(Name = "Serilog - Rolling Interval Day")]
        SerilogRollingIntervalDay,
        [Display(Name = "Serilog - Roll on File Size Limit")]
        SerilogRollOnFileSizeLimit
    }
}
