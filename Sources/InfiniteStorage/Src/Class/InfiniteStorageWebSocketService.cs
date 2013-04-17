using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using WebSocketSharp;
using System.IO;
using log4net;
using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage
{
	class InfiniteStorageWebSocketService: WebSocketService
	{
		private static ILog logger = LogManager.GetLogger("WebsocketService");
		private ProtocolHanlder handler;

		public static event EventHandler<WebsocketEventArgs> DeviceAccepted;
		public static event EventHandler<WebsocketEventArgs> DeviceDisconnected;
		public static event EventHandler<WebsocketEventArgs> PairingRequesting;

		public InfiniteStorageWebSocketService()
		{
			if (!Directory.Exists(MyFileFolder.Temp))
				Directory.CreateDirectory(MyFileFolder.Temp);

			IDirOrganizer organizer = getDirOrganizer();

			var storage = new FlatFileStorage(MyFileFolder.Photo, MyFileFolder.Video, MyFileFolder.Audio, organizer);

			// TODO: remove hard code
			storage.setDeviceName("fakeDevName");

			var ctx = new ProtocolContext(new TempFileFactory(MyFileFolder.Temp), storage, new UnconnectedState()) 
			{
				SendText = this.Send,
				Stop = this.Stop
			};

			ctx.OnConnectAccepted += new EventHandler<WebsocketEventArgs>(ctx_OnConnectAccepted);
			ctx.OnPairingRequired += new EventHandler<WebsocketEventArgs>(ctx_OnPairingRequired);

			handler = new ProtocolHanlder(ctx);
		}

		void ctx_OnPairingRequired(object sender, WebsocketEventArgs e)
		{
			raisePairingRequestingEvent(e);
		}

		void ctx_OnConnectAccepted(object sender, WebsocketEventArgs e)
		{
			raiseDeviceConnectedEvent(e);
		}

		private void raiseDeviceConnectedEvent(WebsocketEventArgs e)
		{
			var handler = DeviceAccepted;
			if (handler != null)
				handler(this, e);
		}

		private void raiseDeviceDisconnectedEvent(WebsocketEventArgs e)
		{
			var handler = DeviceDisconnected;
			if (handler != null)
				handler(this, e);
		}

		private void raisePairingRequestingEvent(WebsocketEventArgs e)
		{
			var handler = PairingRequesting;
			if (handler != null)
				handler(this, e);
		}

		private static IDirOrganizer getDirOrganizer()
		{
			switch ((OrganizeMethod)Settings.Default.OrganizeMethod)
			{
				case OrganizeMethod.Year:
					return new DirOrganizerByYYYY();

				case OrganizeMethod.YearMonth:
					return new DirOrganizerByYYYYMM();

				case OrganizeMethod.YearMonthDay:
					return new DirOrganizerByYYYYMMDD();

				default:
					throw new NotImplementedException();
			}
		}

		protected override void onMessage(object sender, MessageEventArgs e)
		{
			try
			{
				handler.HandleMessage(e);
				base.onMessage(sender, e);
			}
			catch (WebsocketProtocol.ProtocolErrorException err)
			{
				logger.Warn("Protocol error. Close connection.", err);
				cleanupForClose();
				Stop(WebSocketSharp.Frame.CloseStatusCode.PROTOCOL_ERROR, err.Message);
			}
			catch (Exception err)
			{
				logger.Warn("Error handing websocket data", err);
				cleanupForClose();
				Stop(WebSocketSharp.Frame.CloseStatusCode.SERVER_ERROR, err.Message);
			}
		}

		protected override void onOpen(object sender, EventArgs e)
		{
			base.onOpen(sender, e);
		}

		protected override void onError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			logger.Warn("Error occured: " + e.Message);

			cleanupForClose();
		}

		protected override void onClose(object sender, CloseEventArgs e)
		{
			cleanupForClose();
		}

		private void cleanupForClose()
		{
			handler.Clear();

			raiseDeviceDisconnectedEvent(new WebsocketEventArgs((ProtocolContext)handler.ctx));
		}
	}
}
