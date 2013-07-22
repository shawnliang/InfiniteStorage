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

#endregion

namespace Waveface.Client
{
	public partial class WaitForPairingDialog : Window
	{
		private WebSocket m_webSocket;
		private DispatcherTimer m_uiDelayTimer;
		private bool m_closeByApp;
		private DateTime? m_firstRequestTime;
		private ObservableCollection<pairing_request> m_requests = new ObservableCollection<pairing_request>();
		private object m_cs = new object();

		public WaitForPairingDialog()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			m_uiDelayTimer = new DispatcherTimer();
			m_uiDelayTimer.Tick += uiDelayTimer_Tick;
			m_uiDelayTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
			m_uiDelayTimer.Start();

			selectionPanel.Visibility = System.Windows.Visibility.Collapsed;
			deviceList.DataContext = m_requests;

			try
			{
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

		private void uiDelayTimer_Tick(object sender, EventArgs e)
		{
			progressBar.Value += 1;

			lock (m_cs)
			{
				if (m_requests.Count > 0)
				{
					var durationAfterFirstRequest = DateTime.Now - m_firstRequestTime.Value;

					if (durationAfterFirstRequest.TotalSeconds > 5.0)
					{
						if (m_requests.Count == 1)
						{
							var device = m_requests.First();
							WS_accept_reject(device, true);
							MessageBox.Show("Device " + device.device_name + " is successfully added to this PC.");

							m_closeByApp = true;

							Close();
						}
						else
						{
							selectionPanel.Visibility = System.Windows.Visibility.Visible;
							findingDevicePanel.Visibility = System.Windows.Visibility.Collapsed;
						}
					}

				}
				else if (progressBar.Value == progressBar.Maximum)
				{
					var result = MessageBox.Show("Favorite is unable to find a connectable device. Please make sure Favorite is running on your device and the device and this PC are on the same WIFI.\r\n\r\nDo you want to search for available device again?\r\n\r\nPress 'Yes' to search again. Press 'No' to quit pairing mode.", "Unable to find available devices", MessageBoxButton.YesNo);

					if (result == MessageBoxResult.Yes)
					{
						progressBar.Value = 0;
					}
					else
						Close();
				}

			}
			


			//string _AndroidDeviceFound = (string)Application.Current.FindResource("AndroidDeviceFound");
			//string _AllowAutoSync = (string)Application.Current.FindResource("AllowAutoSync");

			//MessageBoxResult _messageBoxResult = MessageBox.Show(string.Format(_AllowAutoSync, m_pairingRequest.device_name), _AndroidDeviceFound, MessageBoxButton.YesNo);

			//if (_messageBoxResult == MessageBoxResult.Yes)
			//{
			//	WS_accept_reject(true);
			//}
			//else
			//{
			//	WS_accept_reject(false);
			//}

			//m_closeByApp = true;

			//Close();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			try
			{
				m_uiDelayTimer.Stop();
				if (!m_closeByApp)
				{
					WS_close_byUser();
				}

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
				deviceList.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(()=>{
					lock (m_cs)
					{
						m_requests.Add(_msgs.pairing_request);
						if (m_requests.Count == 1)
							m_firstRequestTime = DateTime.Now;
					}
				}));
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

		private void WS_accept_reject(pairing_request request, bool accept)
		{
			PairingClientMsgs _msgs = new PairingClientMsgs
				                          {
					                          pairing_mode = new pairing_mode {enabled = false}
				                          };

			if (accept)
			{
				_msgs.accept = new accept_reject { device_id = request.device_id };
			}
			else
			{
				_msgs.reject = new accept_reject { device_id = request.device_id };
			}

			string _json = JsonConvert.SerializeObject(_msgs);

			m_webSocket.Send(_json);
		}

		private void WS_accept_multi(IEnumerable<pairing_request> requests)
		{
			foreach (var req in requests)
			{
				var acceptMsg = new PairingClientMsgs
				{
					accept = new accept_reject { device_id = req.device_id }
				};

				m_webSocket.Send(JsonConvert.SerializeObject(acceptMsg));
			}

			PairingClientMsgs closeMsg = new PairingClientMsgs
			{
				pairing_mode = new pairing_mode { enabled = false }
			};
			m_webSocket.Send(JsonConvert.SerializeObject(closeMsg));
		}

		private void WS_reject_multi(IEnumerable<pairing_request> requests)
		{
			foreach (var req in requests)
			{
				var rejectMsg = new PairingClientMsgs
				{
					reject = new accept_reject { device_id = req.device_id }
				};

				m_webSocket.Send(JsonConvert.SerializeObject(rejectMsg));
			}

			PairingClientMsgs closeMsg = new PairingClientMsgs
			{
				pairing_mode = new pairing_mode { enabled = false }
			};
			m_webSocket.Send(JsonConvert.SerializeObject(closeMsg));
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

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void spGooglePlay_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://play.google.com/store/apps/details?id=com.waveface.uploader");
		}

		private void btnHelp_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(MainWindow.HELP_URL);
		}

		private void Hyperlink_RequestNavigate_1(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start("https://play.google.com/store/apps/details?id=com.waveface.uploader");
		}

		private void AllowSelectedButton_Clicked(object sender, RoutedEventArgs e)
		{
			var selected = deviceList.SelectedItems.OfType<pairing_request>();
			if (selected != null)
			{
				WS_accept_multi(selected);
			}

			m_closeByApp = true;
			Close();
		}

		private void DisallowAllButton_Clicked(object sender, RoutedEventArgs e)
		{
			WS_reject_multi(m_requests);
			m_closeByApp = true;
			Close();
		}
	}
}