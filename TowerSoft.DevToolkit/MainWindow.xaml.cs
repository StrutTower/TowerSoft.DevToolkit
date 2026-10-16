using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TowerSoft.DevToolkit.Services;
using TowerSoft.DevToolkit.Utilities;

namespace TowerSoft.DevToolkit {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            ServiceCollection services = new();
            services.AddWpfBlazorWebView();
            services.AddBlazorWebViewDeveloperTools();
            services.AddSingleton<AppSettingsService>();
           

            Resources.Add("services", services.BuildServiceProvider());
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            WindowsInteropUtilities.SetDarkModeAndTitlebarColor(this);
        }
    }
}
