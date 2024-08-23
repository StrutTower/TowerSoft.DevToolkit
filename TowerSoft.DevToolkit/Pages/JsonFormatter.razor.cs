using System.Text.Json;

namespace TowerSoft.DevToolkit.Pages {
    public partial class JsonFormatter {
        private string inputText;
        private string outputText;
        private bool errorOccurred;

        private async Task Minify() {
            errorOccurred = false;
            try {
                //Built-in async methods do not work string and JsonDocuments
                await Task.Run(() => {
                    using JsonDocument jDoc = JsonDocument.Parse(inputText);
                    outputText = JsonSerializer.Serialize(jDoc);
                });
            } catch (Exception ex) {
                errorOccurred = true;
                outputText = ex.Message;
            }
        }

        private async Task Beautify() {
            errorOccurred = false;
            try {
                //Built-in async methods do not work string and JsonDocuments
                await Task.Run(() => {
                    using JsonDocument jDoc = JsonDocument.Parse(inputText);
                    outputText = JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
                });
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
