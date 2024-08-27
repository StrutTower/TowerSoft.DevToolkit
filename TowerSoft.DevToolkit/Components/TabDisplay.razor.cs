using Microsoft.AspNetCore.Components;

namespace TowerSoft.DevToolkit.Components {
    public partial class TabDisplay {
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string TabID { get; set; }

        [Parameter] public string CurrentTabID { get; set; }
    }
}
