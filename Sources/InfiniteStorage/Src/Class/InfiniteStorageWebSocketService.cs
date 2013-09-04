using InfiniteStorage.Model;
using InfiniteStorage.Notify;
using InfiniteStorage.WebsocketProtocol;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Waveface.Common;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace InfiniteStorage
{
	class InfiniteStorageWebSocketService : WebSocketService
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
				SendFunc = this.Send,
				StopFunc = this.Stop,
				PingFunc = this.Ping
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
			try
			{
				var util = new TransmitUtility();
				var flushed = util.FlushFileRecords(handler.ctx as ProtocolContext);

				notifyAffectedFolder(flushed);

				handler.Clear();
				flushTimer.Stop();
			}
			catch (Exception err)
			{
				logger.Warn("cleanupForClose failed", err);
			}
			finally
			{
				raiseDeviceDisconnectedEvent(new WebsocketEventArgs((ProtocolContext)handler.ctx));
			}
		}

		private static void notifyAffectedFolder(ICollection<FileAsset> flushed)
		{
			var affected_folders = flushed.Select(x => x.parent_folder).Distinct();
			foreach (var folder in affected_folders)
			{
				UIChangeNotificationController.NotifyFolderUpdate(new Folder { name = Path.GetFileName(folder), parent_folder = Path.GetDirectoryName(folder), path = Path.Combine(MyFileFolder.Photo, folder) });
			}
		}

		private void flushFileQueueIfRequired(object nothing)
		{
			try
			{
				var ctx = this.handler.ctx as ProtocolContext;

				var util = new TransmitUtility();
				var flushed = util.FlushFileRecordsIfNoFlushedForXSec(TransmitUtility.BULK_INSERT_BATCH_SECONDS * 2, ctx);


				if (flushed.Any())
					notifyAffectedFolder(flushed);
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("periodically flush file queue failed", err);
			}
		}
	}
}
