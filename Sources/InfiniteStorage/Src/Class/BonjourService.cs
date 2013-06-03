using Mono.Zeroconf;
using System;
using InfiniteStorage.Properties;


namespace InfiniteStorage
{
	class BonjourService
	{
		private const string SVC_TYPE = "_infinite-storage._tcp";

		private RegisterService m_svc;

		public event EventHandler<BonjourErrorEventArgs> Error;

		public BonjourService()
		{
			m_svc = new RegisterService();

			m_svc.Response += new RegisterServiceEventHandler(m_svc_Response);
		}

		void m_svc_Response(object o, RegisterServiceEventArgs args)
		{
			if (!args.IsRegistered)
			{
				var handler = Error;
				if (handler != null)
					handler(this, new BonjourErrorEventArgs { error = args.ServiceError });
			}
		}

		public void Register(ushort backup_port, ushort notify_port, ushort rest_port, string server_id, bool is_accepting = true)
		{
			m_svc.Name = Environment.MachineName;
			m_svc.Port = (short)backup_port;
			m_svc.RegType = SVC_TYPE;
			m_svc.ReplyDomain = "local.";

			var txt = new TxtRecord();
			txt.Add("server_id", server_id);
			txt.Add("ws_port", backup_port.ToString());
			txt.Add("notify_port", notify_port.ToString());
			txt.Add("rest_port", rest_port.ToString());
			txt.Add("version", "1.0");
			txt.Add("service_name", ServiceName);
			txt.Add("waiting_for_pair", is_accepting ? "true" : "false");

			m_svc.TxtRecord = txt;
			m_svc.Register();
		}

		public static string ServiceName
		{
			get
			{
				return Settings.Default.LibraryName;
			}
		}

		public void Dispose()
		{
			m_svc.Dispose();
		}
	}

	class BonjourErrorEventArgs : EventArgs
	{
		public Mono.Zeroconf.ServiceErrorCode error { get; set; }
	}
}
