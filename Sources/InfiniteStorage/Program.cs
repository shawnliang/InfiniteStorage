using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Bonjour;

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

			try
			{
				m_bonjourService = new BonjourService();
				m_bonjourService.Error += new EventHandler<BonjourErrorEventArgs>(m_bonjourService_Error);
				m_bonjourService.Register(7777);
			}
			catch
			{
				MessageBox.Show("Bonjour service is not available", "Error");
				return;
			}

			Application.Run(new Form1());

			m_bonjourService.Unregister();
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
