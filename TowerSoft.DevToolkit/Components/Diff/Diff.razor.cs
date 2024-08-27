using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace TowerSoft.DevToolkit.Components.Diff {
    public partial class Diff {
        private string oldText;
        private string newText;
        private bool wordWrap;

        private bool showInputs = true;
        private SideBySideDiffModel model;

        private void CheckDiff() {
            SideBySideDiffBuilder sideBySideDiffBuilder = new();
            model = sideBySideDiffBuilder.BuildDiffModel(oldText ?? "", newText ?? "");
            showInputs = false;
        }

        private void Reset() {
            model = null;
            showInputs = true;
        }
    }
}
