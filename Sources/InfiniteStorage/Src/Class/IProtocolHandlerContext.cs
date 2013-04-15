using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage
{
	public interface IProtocolHandlerContext
	{
		void handleFileStartCmd(TextCommand cmd);

		void handleFileEndCmd(TextCommand cmd);

		void handleBinaryData(byte[] data);

		void handleConnectCmd(TextCommand cmd);

		void Clear();
	}
}
