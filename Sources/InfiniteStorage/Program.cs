using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Bonjour;
using WebSocketSharp.Server;

namespace InfiniteStorage
{
	static class Program
	{
		static BonjourService m_bonjourService;


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);


			var port = (ushort)13895;
			WebSocketServer<InfiniteStorageWebSocketService> ws_server = null;
			try
			{
				m_bonjourService = new BonjourService();
				m_bonjourService.Error += new EventHandler<BonjourErrorEventArgs>(m_bonjourService_Error);
				m_bonjourService.Register(port);


				var url = string.Format("ws://0.0.0.0:{0}/", port);
				ws_server = new WebSocketSharp.Server.WebSocketServer<InfiniteStorageWebSocketService>(url);
				ws_server.Start();
			}
			catch
			{
				MessageBox.Show("Bonjour service is not available", "Error");
				return;
			}

			Application.Run(new Form1());

			m_bonjourService.Unregister();
			ws_server.Stop();
		}

		static void m_bonjourService_Error(object sender, BonjourErrorEventArgs e)
		{
			MessageBox.Show("Bonjour DNS operation error: " + e.error, "Error");
		}

		static void m_eventManager_ServiceRegistered(DNSSDService service, DNSSDFlags flags, string name, string regtype, string domain)
		{
		}

		static void m_eventManager_OperationFailed(DNSSDService service, DNSSDError error)
		{
			MessageBox.Show("Operation returned an error code " + error, "Error");
		}
	}
}
