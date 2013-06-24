#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using InfiniteStorage.Data.Pairing;
using Microsoft.Win32;
using Newtonsoft.Json;
using WebSocketSharp;

#endregion

namespace Waveface.Client
{
    public partial class WaitForPairingDialog : Window
    {
        private WebSocket m_webSocket;
        private DispatcherTimer m_dispatcherTimer;
        private pairing_request m_pairingRequest;

        public WaitForPairingDialog()
        {
            InitializeComponent();

            m_dispatcherTimer = new DispatcherTimer();
            m_dispatcherTimer.Tick += dispatcherTimer_Tick;
            m_dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var _port = Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "pair_port", 0);

                m_webSocket = new WebSocket("ws://127.0.0.1:" + _port);
                m_webSocket.OnMessage += m_webSocket_OnMessage;
                m_webSocket.Connect();

                ws_subscribe_start();
            }
            catch
            {
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            m_dispatcherTimer.Stop();

            MessageBoxResult _messageBoxResult = MessageBox.Show("Pairing Request", "InfiniteStorage", MessageBoxButton.YesNo);

            if (_messageBoxResult == MessageBoxResult.Yes)
            {
                ws_accept_reject(true);
            }
            else
            {
                ws_accept_reject(false);
            }

            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                m_webSocket.Close();
            }
            catch
            {
            }
        }

        private void m_webSocket_OnMessage(object sender, MessageEventArgs e)
        {
            //Debug
            Application.Current.Dispatcher.Invoke((Action)delegate { MessageBox.Show(e.Data); });

            try
            {
                PairingServerMsgs _msgs = JsonConvert.DeserializeObject<PairingServerMsgs>(e.Data);

                m_pairingRequest = _msgs.pairing_request;

                Application.Current.Dispatcher.Invoke((Action)(() => m_dispatcherTimer.Start()));
            }
            catch
            {
            }
        }

        private void ws_subscribe_start()
        {
            PairingClientMsgs _msgs = new PairingClientMsgs
                                          {
                                              subscribe = new subscribe { pairing = true },
                                              pairing_mode = new pairing_mode { enabled = true }
                                          };


            string _json = JsonConvert.SerializeObject(_msgs);

            m_webSocket.Send(_json);
        }

        private void ws_accept_reject(bool accept)
        {
            PairingClientMsgs _msgs = new PairingClientMsgs
                                          {
                                              pairing_mode = new pairing_mode { enabled = false }
                                          };

            if (accept)
            {
                _msgs.accept = new accept_reject { device_id = m_pairingRequest.device_id };
            }
            else
            {
                _msgs.reject = new accept_reject { device_id = m_pairingRequest.device_id };
            }

            string _json = JsonConvert.SerializeObject(_msgs);

            m_webSocket.Send(_json);
        }
    }
}