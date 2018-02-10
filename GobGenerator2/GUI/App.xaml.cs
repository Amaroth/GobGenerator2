using System.Windows;

namespace GobGenerator2.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mwd = new MainWindow();
            mwd.LoadSettings();
            mwd.Show();
        }
    }
}
