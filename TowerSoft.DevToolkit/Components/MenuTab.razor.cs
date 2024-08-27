using Microsoft.AspNetCore.Components;

namespace TowerSoft.DevToolkit.Components {
    public partial class MenuTab {
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string TabID { get; set; }

        [Parameter] public string CurrentTabID { get; set; }

        [Parameter] public EventCallback<string> OnSetTab { get; set; }

        protected override void OnParametersSet() {
            base.OnParametersSet();
        }

        private async Task SetTab() {
            await OnSetTab.InvokeAsync(TabID);
        }
    }
}
