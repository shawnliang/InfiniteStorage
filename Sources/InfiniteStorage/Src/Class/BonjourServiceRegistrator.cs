using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	class BonjourServiceRegistrator
	{
		private static BonjourServiceRegistrator instance = new BonjourServiceRegistrator();

		private BonjourService bonjour;
		private ushort backup_port;
		private ushort notify_port;
		private ushort rest_port;
		private bool is_accepting;
		private object cs = new object();

		private BonjourServiceRegistrator()
		{
		}
		
		public static BonjourServiceRegistrator Instance
		{
			get { return instance; }
		}

		public void SetPorts(ushort backup, ushort notify, ushort rest)
		{
			backup_port = backup;
			notify_port = notify;
			rest_port = rest;
		}

		public void Register(bool? isAccepting = null)
		{
			lock (cs)
			{
				if (bonjour != null)
				{
					bonjour.Dispose();
					Thread.Sleep(500);
				}

				if (isAccepting.HasValue)
					is_accepting = isAccepting.Value;

				bonjour = new BonjourService();
				bonjour.Error += new EventHandler<BonjourErrorEventArgs>(bonjour_Error);
				bonjour.Register(backup_port, notify_port, rest_port, Settings.Default.ServerId, is_accepting);
			}
		}

		void bonjour_Error(object sender, BonjourErrorEventArgs e)
		{
			log4net.LogManager.GetLogger(GetType()).Warn("bonjour service error: " + e.error.ToString());
		}

		public bool IsAccepting
		{
			get { return is_accepting; }
		}
	}
}
