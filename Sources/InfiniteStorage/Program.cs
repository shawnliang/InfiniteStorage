using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Bonjour;
using WebSocketSharp.Server;
using log4net;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	static class Program
	{
		static BonjourService m_bonjourService;
		static NotifyIcon m_notifyIcon;
		static Form1 m_mainForm;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

			Log4netConfigure.InitLog4net();

			log4net.LogManager.GetLogger("main").Debug("==== program started ====");

			initNotifyIcon();


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

			m_mainForm = new Form1();
			m_mainForm.Show();

			Application.Run();

			m_bonjourService.Unregister();
			ws_server.Stop();
		}

		static void Application_ApplicationExit(object sender, EventArgs e)
		{
			if (m_notifyIcon != null)
				m_notifyIcon.Dispose();
		}

		private static void initNotifyIcon()
		{
			m_notifyIcon = new NotifyIcon();
			m_notifyIcon.Text = Resources.ProductName;
			m_notifyIcon.Icon = Resources.product_icon;
			m_notifyIcon.ContextMenu = new ContextMenu(
				new MenuItem[] {
					new MenuItem("Open", (s,e)=>{
						if (m_mainForm.IsDisposed)
							m_mainForm = new Form1();

						m_mainForm.Show();
						m_mainForm.Activate();
					}),
					new MenuItem("Exit", (s,e)=>{Application.Exit();}),
				});
			m_notifyIcon.Visible = true;
		}

		static void m_bonjourService_Error(object sender, BonjourErrorEventArgs e)
		{
			MessageBox.Show("Bonjour DNS operation error: " + e.error, "Error");
			log4net.LogManager.GetLogger("main").Warn("Bonjour DNS operation error: " + e.error.ToString());
		}
	}
}
