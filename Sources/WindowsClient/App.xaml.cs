#region

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Win32;

#endregion

namespace Waveface.Client
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			var cultureName = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "Culture", "");
			
			if (!string.IsNullOrEmpty(cultureName))
			{
				var cultureInfo = new CultureInfo(cultureName);
				var currentThread = Thread.CurrentThread;

				currentThread.CurrentCulture = cultureInfo;
				currentThread.CurrentUICulture = cultureInfo;

				try
				{
					var resourceFile = @"StringTable." + cultureName + ".xaml";

					var rd = LoadComponent(new Uri(resourceFile, UriKind.Relative)) as ResourceDictionary;

					if (rd != null)
					{
						var existsRD = Resources.MergedDictionaries.FirstOrDefault(item => item.Source.OriginalString.Equals(resourceFile, StringComparison.CurrentCultureIgnoreCase));

						if (existsRD != null)
							Resources.MergedDictionaries.Remove(existsRD);

						Resources.MergedDictionaries.Add(rd);
					}
				}
				catch
				{
				}
			}

			Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof (Timeline), new FrameworkPropertyMetadata {DefaultValue = 30});
		}
	}
}