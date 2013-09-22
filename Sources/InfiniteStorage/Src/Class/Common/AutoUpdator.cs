#region

using System;
using AppLimit.NetSparkle;
using InfiniteStorage.Data;
using InfiniteStorage.Properties;

#endregion

namespace Waveface.Common
{
	public class AutoUpdate
	{
		private Sparkle m_autoUpdator;
		private NetSparkleAppCastItem versionInfo;
		private bool forceUpgrade;

		private static string DEV_VERSION_INFO = "http://cdn.waveface.com/WindowsStation/versioninfo.dev.xml";
		private static string PRO_VERSION_INFO = "http://cdn.waveface.com/WindowsStation/versioninfo.xml";

		public AutoUpdate(bool forceUpgrade)
		{
			m_autoUpdator = new Sparkle(UpdateURL);
			m_autoUpdator.ApplicationIcon = Resources.ProductIcon.ToBitmap();
			m_autoUpdator.ApplicationWindowIcon = Resources.ProductIcon;
			this.forceUpgrade = forceUpgrade;
		}

		public void StartLoop()
		{
			m_autoUpdator.StartLoop(true, TimeSpan.FromHours(5.0));
		}

		public void Stop()
		{
			m_autoUpdator.StopLoop();
		}

		public bool IsUpdateRequired()
		{
			var honorSkippedVersion = !forceUpgrade;

			return m_autoUpdator.IsUpdateRequired(m_autoUpdator.GetApplicationConfig(),
			                                      out versionInfo,
			                                      honorSkippedVersion);
		}

		public void ShowUpdateNeededUI()
		{
			if (versionInfo == null)
				throw new InvalidOperationException("No version info. Call IsUpdateRequired() first");

			m_autoUpdator.ShowUpdateNeededUI(versionInfo, forceUpgrade);
		}

		private static string UpdateURL
		{
			get {
#if DEBUG
				return DEV_VERSION_INFO;
#else
				if (ProgramConfig.ApiBaseUri.ToString().Contains("develop"))
					return DEV_VERSION_INFO;
				else
					return PRO_VERSION_INFO;
#endif
			}
		}
	}
}