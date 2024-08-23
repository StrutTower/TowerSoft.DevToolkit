using System.Text.Json;

namespace TowerSoft.DevToolkit.Pages {
    public partial class JsonFormatter {
        private string inputText;
        private string outputText;
        private bool errorOccurred;

        private void Minify() {
            errorOccurred = false;
            try {
                using JsonDocument jDoc = JsonDocument.Parse(inputText);
                outputText = JsonSerializer.Serialize(jDoc);
            } catch (Exception ex) {
                errorOccurred = true;
                outputText = ex.Message;
            }
        }

        private void Beautify() {
            errorOccurred = false;
            try {
                using JsonDocument jDoc = JsonDocument.Parse(inputText);
                outputText = JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
            } catch (Exception ex) {
                errorOccurred = true;
                outputText = ex.Message;
            }
        }

        private void Copy() {
            System.Windows.Clipboard.SetText(outputText);
        }
    }
}
