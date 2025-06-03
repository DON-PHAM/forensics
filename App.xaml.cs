using System.Windows;
using ImageForensics.Services;
using ImageForensics.ViewModels;

namespace ImageForensics
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize services
            var exifService = new ExifService();

            // Initialize ViewModel
            var mainViewModel = new MainViewModel(exifService);

            // Create and show main window
            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
            mainWindow.Show();
        }
    }
} 