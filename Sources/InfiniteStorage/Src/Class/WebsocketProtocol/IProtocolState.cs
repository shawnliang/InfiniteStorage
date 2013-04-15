using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface IProtocolState
	{
		void handleFileStartCmd(ProtocolContext ctx, TextCommand cmd);
		void handleFileEndCmd(ProtocolContext ctx, TextCommand cmd);
		void handleBinaryData(ProtocolContext ctx, byte[] data);
	}
}
