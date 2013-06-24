using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using InfiniteStorage.Data.Pairing;
using InfiniteStorage.Pair;

namespace InfiniteStorage.Pair
{
	class PairWebSocketService : WebSocketService
	{
		private object cs = new object();
		private List<PairWebSocketService> subscribers = new List<PairWebSocketService>();

		public event EventHandler<PairingModeChangingEventArgs> PairingModeChanging;
		public event EventHandler<NewDeviceAcceptingEventArgs> NewDeviceAccepting;

		protected override void onOpen(object sender, EventArgs e)
		{
		}

		protected override void onMessage(object sender, WebSocketSharp.MessageEventArgs e)
		{
			try
			{
				if (e.Type != WebSocketSharp.Frame.Opcode.TEXT)
					throw new FormatException("only accept text msg");

				PairingClientMsgs msg = null;

				try
				{
					msg = JsonConvert.DeserializeObject<PairingClientMsgs>(e.Data);
				}
				catch (Exception err)
				{
					throw new FormatException(err.Message, err);
				}


				if (msg.subscribe != null && msg.subscribe.pairing)
				{
					addToSubscribers();
				}

				if (msg.pairing_mode != null)
				{
					raisePairingModeChangingEvent(msg.pairing_mode.enabled);
				}

				if (msg.accept != null)
				{
					raiseNewDeviceAcceptingEvent(msg.accept.device_id);
				}
			}
			catch (FormatException err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn(err.Message, err);
				Stop(WebSocketSharp.Frame.CloseStatusCode.INCORRECT_DATA, "Msg format error");
				removeFromSubscribers();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Error handling income msg: " + ((e.Type == WebSocketSharp.Frame.Opcode.TEXT) ? e.Data : e.RawData.ToString()), err);
				Stop(WebSocketSharp.Frame.CloseStatusCode.SERVER_ERROR, "server failed to handle data");
				removeFromSubscribers();
			}
		}

		protected override void onClose(object sender, WebSocketSharp.CloseEventArgs e)
		{
			removeFromSubscribers();
		}

		protected override void onError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			removeFromSubscribers();
		}

		private void addToSubscribers()
		{
			lock (cs)
			{
				if (!subscribers.Contains(this))
					subscribers.Add(this);
			}
		}

		private void removeFromSubscribers()
		{
			lock (cs)
			{
				subscribers.Remove(this);
			}
		}

		private void raiseNewDeviceAcceptingEvent(string device_id)
		{
			var handler = NewDeviceAccepting;
			if (handler != null)
				handler(this, new NewDeviceAcceptingEventArgs(device_id));
		}

		private void raisePairingModeChangingEvent(bool enable)
		{
			var handler = PairingModeChanging;
			if (handler != null)
				handler(this, new PairingModeChangingEventArgs(enable));
		}
	}
}
