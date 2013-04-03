using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using WebSocketSharp;
using System.IO;

namespace InfiniteStorage
{
	class InfiniteStorageWebSocketService: WebSocketService
	{
		ProtocolHanlder handler;

		public InfiniteStorageWebSocketService()
		{
			// TODO: use non-hardcode data
			var userFolder = Environment.GetEnvironmentVariable("USERPROFILE");
			var appFolder = Path.Combine(userFolder, "InfiniteStorage");
			var tempFolder = Path.Combine(appFolder, "temp");
			var deviceFolder = Path.Combine(appFolder, "samsung gt-9300");

			if (!Directory.Exists(tempFolder))
				Directory.CreateDirectory(tempFolder);

			if (!Directory.Exists(deviceFolder))
				Directory.CreateDirectory(deviceFolder);

			handler = new ProtocolHanlder(new TempFileFactory(tempFolder), new FlatFileStorage(deviceFolder));
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
				this.Stop(WebSocketSharp.Frame.CloseStatusCode.PROTOCOL_ERROR, err.Message);
				Stop();
			}
			
		}

		protected override void onOpen(object sender, EventArgs e)
		{
			base.onOpen(sender, e);
		}

		protected override void onError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			handler.Error();
			base.onError(sender, e);
		}

		protected override void onClose(object sender, CloseEventArgs e)
		{
			handler.Error();
			base.onClose(sender, e);
		}
	}
}
