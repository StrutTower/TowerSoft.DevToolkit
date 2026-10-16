using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TowerSoft.DevToolkit.Config;

namespace TowerSoft.DevToolkit.Pages {
    public partial class LogSettings {
        private AppSettings appSettings;
        private ElementReference editAppModal;
        private LogApplication modalApp;
        private bool editApp;

        private ElementReference editLogModal;
        private LogInstance modalLogInstance;
        private string modalLogCurrentAppGuid;
        private bool editLog;

        protected override void OnInitialized() {
            appSettings = AppSettingsServices.AppSettings;
        }

        private void BackClicked() {
            navManager.NavigateTo("/#logs");
        }

        #region App
        private async Task AddApp() {
            modalApp = new() { Guid = Guid.NewGuid().ToString() };
            editApp = false;
            await JS.InvokeVoidAsync("twrOpenModal", editAppModal);
        }

        private async Task EditApp(LogApplication app) {
            modalApp = app;
            editApp = true;
            await JS.InvokeVoidAsync("twrOpenModal", editAppModal);
        }

        private async Task CloseAppModal() {
            await JS.InvokeVoidAsync("twrCloseDialog", editAppModal);
        }

        private async Task AppValidSubmit() {
            AppSettings newAppSettings = AppSettingsServices.AppSettings;
            if (newAppSettings.LogApplications == null) {
                newAppSettings.LogApplications = new() { modalApp };
            } else if (newAppSettings.LogApplications.Any(x => x.Guid == modalApp.Guid)) {
                newAppSettings.LogApplications.Remove(appSettings.LogApplications.Single(x => x.Guid == modalApp.Guid));
                newAppSettings.LogApplications.Add(modalApp);
            } else {
                newAppSettings.LogApplications.Add(modalApp);
            }

            AppSettingsServices.Update(newAppSettings);
            appSettings = newAppSettings;
            modalApp = null;
            await JS.InvokeVoidAsync("twrCloseDialog", editAppModal);
        }
        #endregion

        #region Log
        private async Task AddLog(LogApplication app) {
            modalLogInstance = new() { Guid = Guid.NewGuid().ToString() };
            modalLogCurrentAppGuid = app.Guid;
            editLog = false;
            await JS.InvokeVoidAsync("twrOpenModal", editLogModal);
        }

        private async Task EditLog(LogInstance log) {
            modalLogInstance = log;
            editLog = true;
            await JS.InvokeVoidAsync("twrOpenModal", editLogModal);
        }

        private async Task CloseLogModal() {
            await JS.InvokeVoidAsync("twrCloseDialog", editLogModal);
        }

        private async Task LogValidSubmit() {
            AppSettings newAppSettings = AppSettingsServices.AppSettings;

            LogApplication currentApp = null;

            foreach (LogApplication app in newAppSettings.LogApplications) {
                if (app.Logs != null && app.Logs.Any(x => x.Guid == modalLogInstance.Guid)) {
                    currentApp = app;
                }
            }

            if (currentApp == null) {
                currentApp = newAppSettings.LogApplications.Single(x => x.Guid == modalLogCurrentAppGuid);

                currentApp.Logs ??= [];
                currentApp.Logs.Add(modalLogInstance);
            } else {
                currentApp.Logs.Remove(currentApp.Logs.Single(x => x.Guid == modalLogInstance.Guid));
                currentApp.Logs.Add(modalLogInstance);
            }

            AppSettingsServices.Update(newAppSettings);
            appSettings = newAppSettings;
            modalApp = null;
            await JS.InvokeVoidAsync("twrCloseDialog", editLogModal);
        }
        #endregion
    }
}
