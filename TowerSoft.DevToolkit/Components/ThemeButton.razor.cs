using Microsoft.AspNetCore.Components;

namespace TowerSoft.DevToolkit.Components {
    public partial class ThemeButton {
        [Parameter] public string Theme { get; set; }

        [Parameter] public string CurrentTheme { get; set; }

        [Parameter] public EventCallback<string> OnSetTheme { get; set; }

        private string iconClass;

        protected override void OnParametersSet() {
            if (Theme == "Light") {
                iconClass = "mdi mdi-fw mdi-weather-sunny";
            } else if (Theme == "Dark") {
                iconClass = "mdi mdi-fw mdi-weather-night";
            } else {
                iconClass = "mdi mdi-fw mdi-theme-light-dark";
            }
        }

        private async Task SetTheme() {
            await OnSetTheme.InvokeAsync(Theme.ToLower());
        }
    }
}
