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
		ProtocolContext ctx;

		public ProtocolHanlder(ITempFileFactory tempfileFactory, IFileStorage storage, IProtocolState initialState)
		{
			this.ctx = new ProtocolContext(tempfileFactory, storage, initialState);
		}

		public void HandleMessage(MessageEventArgs e)
		{
			try
			{
				if (e.Type == WebSocketSharp.Frame.Opcode.TEXT)
				{
					TextCommand cmd = parseTextCommand(e);

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

		private static TextCommand parseTextCommand(MessageEventArgs e)
		{
			try
			{
				return Newtonsoft.Json.JsonConvert.DeserializeObject<WebsocketProtocol.TextCommand>(e.Data);
			}
			catch (Exception err)
			{
				throw new ProtocolErrorException("Invalid text command: " + e.Data, err);
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
