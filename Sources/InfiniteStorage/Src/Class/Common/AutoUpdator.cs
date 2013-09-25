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
		private NetSparkleAppCastItem m_versionInfo;
		private bool m_forceUpgrade;

		private static string DEV_VERSION_INFO = "http://cdn.waveface.com/WindowsStation/versioninfo.dev.xml";
		private static string PRO_VERSION_INFO = "http://cdn.waveface.com/WindowsStation/versioninfo.xml";

		public AutoUpdate(bool forceUpgrade)
		{
			m_autoUpdator = new Sparkle(UpdateURL)
								{
									ApplicationIcon = Resources.ProductIcon.ToBitmap(),
									ApplicationWindowIcon = Resources.ProductIcon
								};

			m_forceUpgrade = forceUpgrade;
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
			var honorSkippedVersion = !m_forceUpgrade;

			return m_autoUpdator.IsUpdateRequired(m_autoUpdator.GetApplicationConfig(),
												  out m_versionInfo,
												  honorSkippedVersion);
		}

		public void ShowUpdateNeededUI()
		{
			if (m_versionInfo == null)
				throw new InvalidOperationException("No version info. Call IsUpdateRequired() first");

			m_autoUpdator.ShowUpdateNeededUI(m_versionInfo, m_forceUpgrade);
		}

		private static string UpdateURL
		{
			get
			{
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