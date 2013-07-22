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
							
							var msgTemplate = Application.Current.FindResource("ANS_deviceAdded") as string;
							MessageBox.Show(string.Format(msgTemplate, device.device_name));

							m_closeByApp = true;

							Close();
						}
						else
						{
							if (findingDevicePanel.Visibility == System.Windows.Visibility.Visible)
							{
								selectionPanel.Visibility = System.Windows.Visibility.Visible;
								findingDevicePanel.Visibility = System.Windows.Visibility.Collapsed;

								deviceList.SelectAll();
							}
						}
					}

				}
				else if (progressBar.Value == progressBar.Maximum)
				{
					var msg = Application.Current.FindResource("ANS_deviveNotFound") as string;
					var title = Application.Current.FindResource("ANS_deviveNotFound_title") as string;
					var result = MessageBox.Show(msg, title, MessageBoxButton.YesNo);

					if (result == MessageBoxResult.Yes)
					{
						progressBar.Value = 0;
					}
					else
						Close();
				}

			}
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
				WS_send_accept(req);
			}

			PairingClientMsgs closeMsg = new PairingClientMsgs
			{
				pairing_mode = new pairing_mode { enabled = false }
			};
			m_webSocket.Send(JsonConvert.SerializeObject(closeMsg));
		}

		private void WS_send_accept(pairing_request req)
		{
			var acceptMsg = new PairingClientMsgs
			{
				accept = new accept_reject { device_id = req.device_id }
			};

			m_webSocket.Send(JsonConvert.SerializeObject(acceptMsg));
		}

		private void WS_reject_multi(IEnumerable<pairing_request> requests)
		{
			foreach (var req in requests)
			{
				WS_send_reject(req);
			}

			PairingClientMsgs closeMsg = new PairingClientMsgs
			{
				pairing_mode = new pairing_mode { enabled = false }
			};
			m_webSocket.Send(JsonConvert.SerializeObject(closeMsg));
		}

		private void WS_send_reject(pairing_request req)
		{
			var rejectMsg = new PairingClientMsgs
			{
				reject = new accept_reject { device_id = req.device_id }
			};

			m_webSocket.Send(JsonConvert.SerializeObject(rejectMsg));
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

		private void AllowSelectedButton_Clicked(object sender, RoutedEventArgs e)
		{
			var selected = deviceList.SelectedItems.OfType<pairing_request>();
			var all = deviceList.Items.OfType<pairing_request>();
			var unselected = all.Except(selected);

			if (selected != null)
			{
				foreach (var item in selected)
					WS_send_accept(item);
			}

			if (unselected != null)
			{
				foreach (var item in unselected)
					WS_send_reject(item);
			}

			WS_close_byUser();

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