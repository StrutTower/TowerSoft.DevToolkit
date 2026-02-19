using Microsoft.AspNetCore.Components;

namespace TowerSoft.DevToolkit.Components.Icons {
    public abstract partial class IconBase {
        private readonly string additionalClasses;

        public IconBase() { }

        public IconBase(string additionalClasses) {
            this.additionalClasses = additionalClasses;
        }

        protected abstract List<string> Paths { get; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }


        protected override void OnParametersSet() {
            if (Attributes == null) Attributes = [];

            string classList = "tabler-icon";
            if (!string.IsNullOrWhiteSpace(additionalClasses))
                classList += " " + additionalClasses;

            if (Attributes.TryGetValue("class", out var classes)) {
                classList += " " + classes;
                Attributes["class"] = classList;
            } else {
                Attributes.Add("class", classList);
            }
        }
    }
}
