using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage
{
	public class ProtocolHanlder
	{
		TransmitContext ctx;

		public ProtocolHanlder(ITempFileFactory tempfileFactory, IFileStorage storage)
		{
			this.ctx = new TransmitContext(tempfileFactory, storage);
		}

		public void HandleMessage(MessageEventArgs e)
		{
			try
			{
				if (e.Type == WebSocketSharp.Frame.Opcode.TEXT)
				{
					var cmd = Newtonsoft.Json.JsonConvert.DeserializeObject<WebsocketProtocol.TextCommand>(e.Data);

					if (cmd.isFileStartCmd())
						ctx.handleFileStartCmd(cmd);
					else if (cmd.isFileEndCmd())
						ctx.handleFileEndCmd(cmd);
					else
						throw new ProtocolErrorException("Unknown action: " + cmd.action);
				}
				else if (e.Type == WebSocketSharp.Frame.Opcode.BINARY)
				{
					ctx.handleBinaryData(e.RawData);
				}
			}
			catch
			{
				OnError();
				throw;
			}
		}

		public void Clear()
		{
			if (ctx != null && ctx.temp_file != null)
			{
				ctx.temp_file.Delete();
				ctx.temp_file = null;
			}
		}

		public void OnError()
		{
			Clear();
		}
	}
}
