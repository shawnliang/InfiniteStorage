using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bonjour;

namespace InfiniteStorage
{
	class BonjourService
	{
		Bonjour.DNSSDService m_service;
		Bonjour.DNSSDService m_register;
		Bonjour.DNSSDEventManager m_eventManager;

		public event EventHandler<BonjourErrorEventArgs> Error;

		public BonjourService()
		{
			m_service = new DNSSDService();
			m_eventManager = new DNSSDEventManager();
			m_eventManager.ServiceRegistered += new _IDNSSDEvents_ServiceRegisteredEventHandler(m_eventManager_ServiceRegistered);
			m_eventManager.OperationFailed += new _IDNSSDEvents_OperationFailedEventHandler(m_eventManager_OperationFailed);
		}

		void m_eventManager_OperationFailed(DNSSDService service, DNSSDError error)
		{
			var handler = Error;
			if (handler != null)
				handler(this, new BonjourErrorEventArgs { error = error });
		}

		void m_eventManager_ServiceRegistered(DNSSDService service, DNSSDFlags flags, string name, string regtype, string domain)
		{
		}

		public void Register(ushort port)
		{
			var txt = new TXTRecord();
			txt.SetValue("ws_port", Encoding.UTF8.GetBytes(port.ToString()));
			txt.SetValue("version", Encoding.UTF8.GetBytes("1.0"));
			m_register = m_service.Register(0, 0, Environment.UserName, "_infinite-storage._tcp", null, null, port, txt, m_eventManager);
		}

		public void Unregister()
		{
			if (m_register != null)
				m_register.Stop();
		}
	}

	class BonjourErrorEventArgs : EventArgs
	{
		public DNSSDError error { get; set; }
	}
}
