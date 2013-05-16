using System.Windows;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		void OnApplicationStartup(object sender, StartupEventArgs args)
		{
			MainWindow mainWindow = new MainWindow();
			mainWindow.Show();
		}
	}
}
