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
        private DispatcherTimer m_uiDelayTimer;
        private pairing_request m_pairingRequest;
        private bool m_closeByApp;

        public WaitForPairingDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_uiDelayTimer = new DispatcherTimer();
            m_uiDelayTimer.Tick += uiDelayTimer_Tick;
            m_uiDelayTimer.Interval = new TimeSpan(0, 0, 1);

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
            m_uiDelayTimer.Stop();

            MessageBoxResult _messageBoxResult = MessageBox.Show("Pairing Request - [" + m_pairingRequest.device_name + "]", "Favorite*", MessageBoxButton.YesNo);

            if (_messageBoxResult == MessageBoxResult.Yes)
            {
                WS_accept_reject(true);
            }
            else
            {
                WS_accept_reject(false);
            }

            m_closeByApp = true;

            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
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

                m_pairingRequest = _msgs.pairing_request;

                Application.Current.Dispatcher.Invoke((Action)(() => m_uiDelayTimer.Start()));
            }
            catch
            {
            }
        }

        private void WS_subscribe_start()
        {
            PairingClientMsgs _msgs = new PairingClientMsgs
                                          {
                                              subscribe = new subscribe { pairing = true },
                                              pairing_mode = new pairing_mode { enabled = true }
                                          };


            string _json = JsonConvert.SerializeObject(_msgs);

            m_webSocket.Send(_json);
        }

        private void WS_accept_reject(bool accept)
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

        private void WS_close_byUser()
        {
            PairingClientMsgs _msgs = new PairingClientMsgs
                                          {
                                              pairing_mode = new pairing_mode { enabled = false }
                                          };

            string _json = JsonConvert.SerializeObject(_msgs);

            m_webSocket.Send(_json);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void spGooglePlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ;
        }
    }
}