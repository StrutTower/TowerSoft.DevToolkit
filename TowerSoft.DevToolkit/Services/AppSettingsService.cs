using Newtonsoft.Json;
using System.IO;
using TowerSoft.DevToolkit.Config;

namespace TowerSoft.DevToolkit.Services {
    public class AppSettingsService {
        private AppSettings appSettings;
        private string settingsPath =
            Environment.ExpandEnvironmentVariables(@"%APPDATA%\TowerSoft\DevToolkit\settings.json");

        public AppSettingsService() {
            if (File.Exists(settingsPath)) {
                appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(settingsPath));
            } else {
                FileInfo fi = new(settingsPath);

                if (!Directory.Exists(fi.Directory.FullName))
                    Directory.CreateDirectory(fi.Directory.FullName);

                appSettings = GetDefaultSettings();
                File.WriteAllText(settingsPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
            }
        }

        public AppSettings AppSettings {
            get => appSettings;
        }

        public void Update(AppSettings newAppSettings) {
            appSettings = newAppSettings;
            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(newAppSettings, Formatting.Indented));
        }

        private static AppSettings GetDefaultSettings() {
            return new() {
                GitScannerDefaultPath = @"%USERPROFILE%\source\repos"
            };
        }
    }
}
