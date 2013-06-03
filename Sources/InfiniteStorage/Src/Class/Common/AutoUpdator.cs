using AppLimit.NetSparkle;
using System;

namespace Waveface.Common
{
	public class AutoUpdate
	{
		private Sparkle m_autoUpdator;
		private NetSparkleAppCastItem versionInfo;
		private bool forceUpgrade;

		public AutoUpdate(bool forceUpgrade)
		{
			m_autoUpdator = new Sparkle(UpdateURL);
			//m_autoUpdator.ApplicationIcon = Resources.software_update_available;
			//m_autoUpdator.ApplicationWindowIcon = Resources.UpdateAvailable;

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
			get
			{
				return "https://waveface.com/extensions/bunnyUpdate/versioninfo.xml";
			}
		}
	}
}
