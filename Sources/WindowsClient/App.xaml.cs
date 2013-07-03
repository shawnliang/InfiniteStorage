using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Media.Animation;

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
					var resourceFile = @"StringTable." + cultureName + ".xaml";

					var rd = Application.LoadComponent(new Uri(resourceFile, UriKind.Relative)) as ResourceDictionary;

					if (rd != null)
					{
						var existsRD = this.Resources.MergedDictionaries.Where(item => item.Source.OriginalString.Equals(resourceFile, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

						if (existsRD != null)
							this.Resources.MergedDictionaries.Remove(existsRD);
						
						this.Resources.MergedDictionaries.Add(rd);
					}
				}
				catch
				{
				}
			}

			Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });
		}
	}
}
