using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using WebSocketSharp;
using System.IO;
using log4net;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	class InfiniteStorageWebSocketService: WebSocketService
	{
		private static ILog logger = LogManager.GetLogger("WebsocketService");
		private ProtocolHanlder handler;

		public InfiniteStorageWebSocketService()
		{
			if (!Directory.Exists(MyFileFolder.Temp))
				Directory.CreateDirectory(MyFileFolder.Temp);

			var storage = new FlatFileStorage(MyFileFolder.Photo, MyFileFolder.Video, MyFileFolder.Audio);

			// TODO: remove hard code
			storage.setDeviceName("fakeDevName");

			handler = new ProtocolHanlder(new TempFileFactory(MyFileFolder.Temp), storage);
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
				Stop(WebSocketSharp.Frame.CloseStatusCode.PROTOCOL_ERROR, err.Message);
			}
			catch (Exception err)
			{
				logger.Warn("Error handing websocket data", err);
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
			handler.OnError();
			base.onError(sender, e);
		}

		protected override void onClose(object sender, CloseEventArgs e)
		{
			handler.Clear();
			base.onClose(sender, e);
		}
	}
}
