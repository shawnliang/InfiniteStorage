using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using WebSocketSharp;
using log4net;

namespace InfiniteStorage.Notify
{
	class NotifyWebSocketService : WebSocketService
	{
		public static event EventHandler<NotifyChannelEventArgs> Subscribing;
		public static event EventHandler<NotifyChannelEventArgs> Disconnected;


		private static ILog logger = LogManager.GetLogger("notify");
		private NotifyChannelHandler handler = new NotifyChannelHandler();

		public NotifyWebSocketService()
		{
			handler.Subscribing += handler_Subscribing;
		}

		protected override void onMessage(object sender, MessageEventArgs e)
		{
			try
			{
				if (e.Type == WebSocketSharp.Frame.Opcode.TEXT)
					logger.Debug(e.Data);

				handler.HandleMessage(this, e);
			}
			catch (WebsocketProtocol.ProtocolErrorException err)
			{
				logger.Warn("protocol error", err);
				closeAndFireEvent(WebSocketSharp.Frame.CloseStatusCode.PROTOCOL_ERROR, err.Message);
			}
			catch (Exception err)
			{
				logger.Warn("error", err);
				closeAndFireEvent(WebSocketSharp.Frame.CloseStatusCode.SERVER_ERROR, err.Message);
			}
		}

		protected override void onClose(object sender, CloseEventArgs e)
		{
			logger.Debug("connection closed: " + e.Reason);
			raiseDisconnectedEvent();
		}

		protected override void onError(object sender, ErrorEventArgs e)
		{
			logger.Warn("connection error: " + e.Message);
			closeAndFireEvent(WebSocketSharp.Frame.CloseStatusCode.SERVER_ERROR, e.Message);
		}

		protected override void onOpen(object sender, EventArgs e)
		{
			logger.Debug("Client connected");
		}

		private void closeAndFireEvent(WebSocketSharp.Frame.CloseStatusCode code, string reason)
		{
			Stop(code, reason);
			raiseDisconnectedEvent();
		}

		void handler_Subscribing(object sender, NotifyChannelEventArgs e)
		{
			var evtHandler = Subscribing;
			if (evtHandler != null)
				evtHandler(sender, e);
		}

		void raiseDisconnectedEvent()
		{
			if (handler.Ctx != null)
			{
				var evtHandler = Disconnected;
				if (evtHandler != null)
					evtHandler(this, new NotifyChannelEventArgs { Ctx = handler.Ctx });
			}
		}
	}
}
