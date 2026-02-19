using Microsoft.AspNetCore.Components;
using System.Web;

namespace TowerSoft.DevToolkit.Components.Icons {
    public abstract partial class IconAdvancedBase {
        private readonly string additionalClasses;

        public IconAdvancedBase() { }

        public IconAdvancedBase(string additionalClasses) {
            this.additionalClasses = additionalClasses;
        }

        protected abstract List<ISvgSubElement> Parts { get; }

        [Parameter]
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

    public interface ISvgSubElement { 
        string ElementName { get; }

        public MarkupString ToHtml();
    }

    public class SvgPath(string d) : ISvgSubElement {
        public string ElementName => "path";

        public string D { get; } = d;

        public MarkupString ToHtml() {
            return new MarkupString($"<{ElementName} d=\"{D}\" />");
        }

        public static SvgPath New(string d) {
            return new SvgPath(d);
        }
    }

    public class SvgRect(int x, int y, int width, int height) : ISvgSubElement {
        public string ElementName => "rect";

        public int X { get; } = x;
        public int Y { get; } = y;
        public int Width { get; } = width;
        public int Height { get; } = height;

        public MarkupString ToHtml() {
            return new MarkupString($"<{ElementName} x=\"{X}\" y=\"{Y}\" width=\"{Width}\" height=\"{Height}\" />");
        }

        public static SvgRect New(int x, int y, int width, int height) {
            return new SvgRect(x, y, width, height);
        }
    }
}
