using Microsoft.AspNetCore.Components;

namespace TowerSoft.DevToolkit.Components {
    public partial class ThemeButton {
        [Parameter] public string Theme { get; set; }

        [Parameter] public string CurrentTheme { get; set; }

        [Parameter] public EventCallback<string> OnSetTheme { get; set; }

        private async Task SetTheme() {
            await OnSetTheme.InvokeAsync(Theme.ToLower());
        }
    }
}
