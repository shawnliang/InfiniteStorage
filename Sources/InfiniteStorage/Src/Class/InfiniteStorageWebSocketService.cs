#region

using System;
using System.Diagnostics;
using System.IO;
using InfiniteStorage.WebsocketProtocol;
using Waveface.Common;
using WebSocketSharp;
using WebSocketSharp.Frame;
using WebSocketSharp.Server;
using log4net;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

#endregion

namespace InfiniteStorage
{
	internal class InfiniteStorageWebSocketService : WebSocketService
	{
		private static ILog logger = LogManager.GetLogger("WebsocketService");
		private ProtocolHanlder handler;
		private NoReentrantTimer flushTimer;

		public static event EventHandler<WebsocketEventArgs> DeviceAccepted;
		public static event EventHandler<WebsocketEventArgs> DeviceDisconnected;
		public static event EventHandler<WebsocketEventArgs> PairingRequesting;
		public static event EventHandler<WebsocketEventArgs> TotalCountUpdated;
		public static event EventHandler<WebsocketEventArgs> FileReceiving;
		public static event EventHandler<WebsocketEventArgs> FileProgress;
		public static event EventHandler<WebsocketEventArgs> FileEnding;
		public static event EventHandler<WebsocketEventArgs> FileReceived;
		public static event EventHandler<WebsocketEventArgs> FileDropped;
		public static event EventHandler<ThumbnailReceivedEventArgs> ThumbnailReceived;

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
					          SendFunc = Send,
					          StopFunc = Stop,
					          PingFunc = Ping
				          };

			ctx.OnConnectAccepted += DeviceAccepted;
			ctx.OnPairingRequired += PairingRequesting;
			ctx.OnTotalCountUpdated += TotalCountUpdated;
			ctx.OnFileReceiving += FileReceiving;
			ctx.OnFileProgress += FileProgress;
			ctx.OnFileReceived += FileReceived;
			ctx.OnFileEnding += FileEnding;
			ctx.OnFileDropped += FileDropped;
			ctx.OnThumbnailReceived += ThumbnailReceived;

			ctx.SetData(TransmitUtility.BULK_INSERT_QUEUE_CS, new object());

			handler = new ProtocolHanlder(ctx);

			flushTimer = new NoReentrantTimer(flushFileQueueIfRequired, null, 4000, 4000);
			flushTimer.Start();
		}

		private void raiseDeviceDisconnectedEvent(WebsocketEventArgs e)
		{
			var _handler = DeviceDisconnected;

			if (_handler != null)
				_handler(this, e);
		}

		protected override void onMessage(object sender, MessageEventArgs e)
		{
			try
			{
				var sw = new Stopwatch();
				sw.Start();
				handler.HandleMessage(e);
				sw.Stop();

				if (e.Type == Opcode.TEXT)
					logger.Debug("proc cmd : " + sw.ElapsedMilliseconds + " ms");

				base.onMessage(sender, e);
			}
			catch (ProtocolErrorException err)
			{
				logger.Warn("Protocol error. Close connection.", err);
				cleanupForClose();
				Stop(CloseStatusCode.PROTOCOL_ERROR, err.Message);
			}
			catch (Exception err)
			{
				logger.Warn("Error handing websocket data", err);
				cleanupForClose();
				Stop(CloseStatusCode.SERVER_ERROR, err.Message);
			}
		}

		protected override void onOpen(object sender, EventArgs e)
		{
			base.onOpen(sender, e);
		}

		protected override void onError(object sender, ErrorEventArgs e)
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
			try
			{
				var util = new TransmitUtility();
				util.FlushFileRecords(handler.ctx as ProtocolContext);

				handler.Clear();
				flushTimer.Stop();
			}
			catch (Exception err)
			{
				logger.Warn("cleanupForClose failed", err);
			}
			finally
			{
				raiseDeviceDisconnectedEvent(new WebsocketEventArgs((ProtocolContext) handler.ctx));
			}
		}

		private void flushFileQueueIfRequired(object nothing)
		{
			try
			{
				var ctx = handler.ctx as ProtocolContext;

				var util = new TransmitUtility();
				util.FlushFileRecordsIfNoFlushedForXSec(TransmitUtility.BULK_INSERT_BATCH_SECONDS*2, ctx);
			}
			catch (Exception err)
			{
				LogManager.GetLogger(GetType()).Warn("periodically flush file queue failed", err);
			}
		}
	}
}