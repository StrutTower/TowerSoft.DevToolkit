using NUglify;
using NUglify.Css;
using NUglify.Html;
using NUglify.JavaScript;
using System.Text.Json;
using TowerSoft.DevToolkit.Models;

namespace TowerSoft.DevToolkit.Components.Formatter {
    public partial class Formatter {
        private string inputText;
        private string outputText;
        private bool errorOccurred;
        private FormatterLanguageType languageType;

        private async Task Minify() {
            FormatterResult result;
            switch (languageType) {
                case FormatterLanguageType.HTML:
                    result = await FormatHtml(true);
                    break;
                case FormatterLanguageType.CSS:
                    result = await FormatCss(true);
                    break;
                case FormatterLanguageType.JavaScript:
                    result = await FormatJs(true);
                    break;
                case FormatterLanguageType.JSON:
                    result = await FormatJson(true);
                    break;
                default:
                    return;
            }
            outputText = result.Output;
            errorOccurred = result.ErrorOccurred;
        }

        private async Task Beautify() {
            FormatterResult result;
            switch (languageType) {
                case FormatterLanguageType.HTML:
                    result = await FormatHtml(false);
                    break;
                case FormatterLanguageType.CSS:
                    result = await FormatCss(false);
                    break;
                case FormatterLanguageType.JavaScript:
                    result = await FormatJs(false);
                    break;
                case FormatterLanguageType.JSON:
                    result = await FormatJson(false);
                    break;
                default:
                    return;
            }
            outputText = result.Output;
            errorOccurred = result.ErrorOccurred;
        }

        private void Copy() {
            System.Windows.Clipboard.SetText(outputText);
        }

        private async Task<FormatterResult> FormatHtml(bool minify) {
            HtmlSettings settings = new();
            if (minify) {
                settings.KeepTags.Add("html");
                settings.KeepTags.Add("head");
                settings.KeepTags.Add("body");
            } else {
                settings.PrettyPrint = true;
                settings.Indent = "    ";
                settings.RemoveOptionalTags = false;
            }

            UglifyResult uglifyResult = new();
            await Task.Run(() => {
                uglifyResult = Uglify.Html(inputText, settings);
            });

            if (uglifyResult.Errors.Count == 0) {
                return new() {
                    Output = uglifyResult.Code
                };
            }

            return new() {
                ErrorOccurred = true,
                Output = string.Join(Environment.NewLine, uglifyResult.Errors.Select(x => x.ToString()))
            };
        }

        private async Task<FormatterResult> FormatCss(bool minify) {
            CssSettings settings = new();
            if (!minify) {
                settings.OutputMode = OutputMode.MultipleLines;
                settings.Indent = "    ";
            }

            UglifyResult uglifyResult = new();
            await Task.Run(() => {
                uglifyResult = Uglify.Css(inputText, settings);
            });

            if (uglifyResult.Errors.Count == 0) {
                return new() {
                    Output = uglifyResult.Code
                };
            }

            return new() {
                ErrorOccurred = true,
                Output = string.Join(Environment.NewLine, uglifyResult.Errors.Select(x => x.ToString()))
            };
        }

        private async Task<FormatterResult> FormatJs(bool minify) {
            CodeSettings settings = new();
            if (!minify) {
                settings.OutputMode = OutputMode.MultipleLines;
                settings.Indent = "    ";
            }

            UglifyResult uglifyResult = new();
            await Task.Run(() => {
                uglifyResult = Uglify.Js(inputText, settings);
            });

            if (uglifyResult.Errors.Count == 0) {
                return new() {
                    Output = uglifyResult.Code
                };
            }

            return new() {
                ErrorOccurred = true,
                Output = string.Join(Environment.NewLine, uglifyResult.Errors.Select(x => x.ToString()))
            };
        }

        private async Task<FormatterResult> FormatJson(bool minify) {
            JsonSerializerOptions options = new();
            if (!minify)
                options.WriteIndented = true;

            FormatterResult result = new();
            try {
                //Built-in async methods do not work strings and JsonDocuments
                await Task.Run(() => {
                    using JsonDocument jDoc = JsonDocument.Parse(inputText);
                    result.Output = JsonSerializer.Serialize(jDoc, options);
                });
            } catch (Exception ex) {
                result.ErrorOccurred = true;
                result.Output = ex.Message;
            }
            return result;
        }
    }
}
