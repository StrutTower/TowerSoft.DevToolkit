using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;
using TowerSoft.DevToolkit.Utilities;

namespace TowerSoft.DevToolkit {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            serviceCollection.AddBlazorWebViewDeveloperTools();

            Resources.Add("services", serviceCollection.BuildServiceProvider());
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            WindowsInteropUtilities.SetDarkModeAndTitlebarColor(this);
        }
    }
}
