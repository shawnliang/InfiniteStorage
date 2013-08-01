#region

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using InfiniteStorage.Data;
using Microsoft.Win32;
using Waveface.Client.Properties;

#endregion

namespace Waveface.Client
{
	public partial class App : Application
	{
		InfiniteStorage.Win32.MessageReceiver m_wndMsgRecver;

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

			m_wndMsgRecver = new InfiniteStorage.Win32.MessageReceiver(IPCData.UI_CLASS_NAME, null);
			m_wndMsgRecver.AllowMessage((uint)NativeMethods.WM_COPYDATA);
			m_wndMsgRecver.WndProc += m_wndMsgRecver_WndProc;

			if (!Settings.Default.IsUpgraded)
			{
				Settings.Default.Upgrade();
				Settings.Default.IsUpgraded = true;
				Settings.Default.Save();
			}

		}

		void m_wndMsgRecver_WndProc(object sender, InfiniteStorage.Win32.MessageEventArgs e)
		{
			try
			{
				if (e.Message == NativeMethods.WM_COPYDATA)
				{
					var data = (NativeMethods.COPYDATASTRUCT)Marshal.PtrToStructure(e.lParam, typeof(NativeMethods.COPYDATASTRUCT));

					switch ((CopyDataType)data.dwData)
					{
						case CopyDataType.JUMP_TO_DEVICE_NODE:
							var window = (MainWindow)Application.Current.MainWindow;
							window.JumpToDevice(data.lpData);
							break;
						default:
							break;
					}
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Process windows message error", err);
			}
		}
	}
}