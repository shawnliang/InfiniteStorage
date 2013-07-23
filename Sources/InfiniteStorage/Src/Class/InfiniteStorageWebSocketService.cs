using InfiniteStorage.WebsocketProtocol;
using log4net;
using System;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;

namespace InfiniteStorage
{
	class InfiniteStorageWebSocketService : WebSocketService
	{
		private static ILog logger = LogManager.GetLogger("WebsocketService");
		private ProtocolHanlder handler;

		public static event EventHandler<WebsocketEventArgs> DeviceAccepted;
		public static event EventHandler<WebsocketEventArgs> DeviceDisconnected;
		public static event EventHandler<WebsocketEventArgs> PairingRequesting;
		public static event EventHandler<WebsocketEventArgs> TotalCountUpdated;
		public static event EventHandler<WebsocketEventArgs> FileReceiving;
		public static event EventHandler<WebsocketEventArgs> FileEnding;
		public static event EventHandler<WebsocketEventArgs> FileReceived;

		public InfiniteStorageWebSocketService()
		{
			if (!Directory.Exists(MyFileFolder.Temp))
			{
				Directory.CreateDirectory(MyFileFolder.Temp);
				var dir = new DirectoryInfo(MyFileFolder.Temp);
				dir.Attributes = FileAttributes.Hidden | dir.Attributes;
			}

			var storage = new ByMonthFileStorage();

			var ctx = new ProtocolContext(new TempFileFactory(MyFileFolder.Temp), storage, new UnconnectedState())
			{
				SendFunc = this.Send,
				StopFunc = this.Stop,
				PingFunc = this.Ping
			};

			ctx.OnConnectAccepted += DeviceAccepted;
			ctx.OnPairingRequired += PairingRequesting;
			ctx.OnTotalCountUpdated += TotalCountUpdated;
			ctx.OnFileReceiving += FileReceiving;
			ctx.OnFileReceived += FileReceived;
			ctx.OnFileEnding += FileEnding;

			handler = new ProtocolHanlder(ctx);
		}

		private void raiseDeviceDisconnectedEvent(WebsocketEventArgs e)
		{
			var handler = DeviceDisconnected;
			if (handler != null)
				handler(this, e);
		}

		protected override void onMessage(object sender, MessageEventArgs e)
		{
			try
			{
				var sw = new Stopwatch();
				sw.Start();
				handler.HandleMessage(e);
				sw.Stop();

				if (e.Type == WebSocketSharp.Frame.Opcode.TEXT)
					logger.Debug("proc cmd : " + sw.ElapsedMilliseconds + " ms");

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
