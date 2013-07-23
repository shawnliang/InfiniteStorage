#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using InfiniteStorage.Data.Pairing;
using Microsoft.Win32;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Waveface.ClientFramework;

#endregion

namespace Waveface.Client
{
	public partial class WaitForPairingDialog : Window
	{
		private WebSocket m_webSocket;
		private BackgroundWorker m_bgworker = new BackgroundWorker();

		public WaitForPairingDialog()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				digit1.Content = digit2.Content = digit3.Content = digit4.Content = "";

				m_bgworker.DoWork += m_bgworker_DoWork;
				m_bgworker.RunWorkerCompleted += m_bgworker_RunWorkerCompleted;
				m_bgworker.RunWorkerAsync();

				var _port = Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "pair_port", 0);

				m_webSocket = new WebSocket("ws://127.0.0.1:" + _port);
				m_webSocket.OnMessage += webSocket_OnMessage;
				m_webSocket.Connect();

				WS_subscribe_start();
			}
			catch
			{
			}
		}

		void m_bgworker_DoWork(object sender, DoWorkEventArgs e)
		{
			var passcode =StationAPI.QueryPairingPasscode();
			if (passcode.Length != 4)
				throw new FormatException("passcode format is not correct");

			e.Result = passcode;
		}

		void m_bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null || e.Cancelled || e.Result == null)
				return;

			var passcode = e.Result as string;
			digit1.Content = passcode.Substring(0, 1);
			digit2.Content = passcode.Substring(1, 1);
			digit3.Content = passcode.Substring(2, 1);
			digit4.Content = passcode.Substring(3, 1);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			try
			{
				WS_close_byUser();
				m_webSocket.Close();
			}
			catch
			{
			}
		}

		private void webSocket_OnMessage(object sender, MessageEventArgs e)
		{
			try
			{
				PairingServerMsgs _msgs = JsonConvert.DeserializeObject<PairingServerMsgs>(e.Data);
			}
			catch
			{
			}
		}

		private void WS_subscribe_start()
		{
			PairingClientMsgs _msgs = new PairingClientMsgs
				                          {
					                          subscribe = new subscribe {pairing = true},
					                          pairing_mode = new pairing_mode {enabled = true}
				                          };


			string _json = JsonConvert.SerializeObject(_msgs);

			m_webSocket.Send(_json);
		}

		private void WS_close_byUser()
		{
			PairingClientMsgs _msgs = new PairingClientMsgs
				                          {
					                          pairing_mode = new pairing_mode {enabled = false}
				                          };

			string _json = JsonConvert.SerializeObject(_msgs);

			m_webSocket.Send(_json);
		}

		private void btnHelp_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(MainWindow.HELP_URL);
		}

		private void Hyperlink_RequestNavigate_1(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start("https://play.google.com/store/apps/details?id=com.waveface.uploader");
		}
	}
}
