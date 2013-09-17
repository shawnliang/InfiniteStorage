#region

using System;
using Mono.Zeroconf;

#endregion

namespace BonjourAgent
{
	public class BonjourService : IDisposable
	{
		private const string SVC_TYPE = "_infinite-storage._tcp";

		private RegisterService m_svc;

		public event EventHandler<BonjourErrorEventArgs> Error;

		public BonjourService(string serviceName)
		{
			ServiceName = serviceName;

			m_svc = new RegisterService();
			m_svc.Response += m_svc_Response;
		}

		private void m_svc_Response(object o, RegisterServiceEventArgs args)
		{
			if (!args.IsRegistered)
			{
				var handler = Error;

				if (handler != null)
					handler(this, new BonjourErrorEventArgs { error = args.ServiceError });
			}
		}

		public void Register(ushort backup_port, ushort notify_port, ushort rest_port, string server_id, bool is_accepting, bool home_sharing, string passcode)
		{
			m_svc.Name = Environment.MachineName;
			m_svc.Port = (short)backup_port;
			m_svc.RegType = SVC_TYPE;
			m_svc.ReplyDomain = "local.";

			var txt = new TxtRecord
				          {
					          {"server_id", server_id},
					          {"ws_port", backup_port.ToString()},
					          {"notify_port", notify_port.ToString()},
					          {"rest_port", rest_port.ToString()},
					          {"version", "1.0"},
					          {"service_name", ServiceName},
					          {"passcode", passcode},
					          {"waiting_for_pair", is_accepting ? "true" : "false"},
					          {"home_sharing", home_sharing ? "true" : "false"}
				          };

			m_svc.TxtRecord = txt;
			m_svc.Register();
		}

		public string ServiceName { get; set; }

		public void Dispose()
		{
			m_svc.Dispose();
		}
	}

	public class BonjourErrorEventArgs : EventArgs
	{
		public ServiceErrorCode error { get; set; }
	}
}