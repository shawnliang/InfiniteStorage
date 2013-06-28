using System;
using System.Globalization;
using System.Windows;
using Microsoft.Win32;
using System.Threading;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{

			var cultureName = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "Culture", "");
			if (!string.IsNullOrEmpty(cultureName))
			{
				var cultureInfo = new CultureInfo(cultureName);
				var currentThread = Thread.CurrentThread;

				currentThread.CurrentCulture = cultureInfo;
				currentThread.CurrentUICulture = cultureInfo;

				try
				{
					var rd = Application.LoadComponent(new Uri(@"StringTable." + cultureName + ".xaml ", UriKind.Relative)) as ResourceDictionary;

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
}
