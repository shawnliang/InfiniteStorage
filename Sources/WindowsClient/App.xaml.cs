using System;
using System.Globalization;
using System.Windows;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			CultureInfo currentCultureInfo = CultureInfo.CurrentCulture;
			try
			{
				var rd = Application.LoadComponent(new Uri(@"StringTable." + currentCultureInfo.Name + ".xaml ", UriKind.Relative)) as ResourceDictionary;

				if (rd != null)
				{
					this.Resources.MergedDictionaries.Clear();
					this.Resources.MergedDictionaries.Add(rd);
				}
			}
			catch
			{
			}
		}
	}
}
