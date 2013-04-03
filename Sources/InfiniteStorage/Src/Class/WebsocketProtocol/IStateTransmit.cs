using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	interface IStateTransmit
	{
		void handleFileStartCmd(TransmitContext ctx, TextCommand cmd);
		void handleFileEndCmd(TransmitContext ctx, TextCommand cmd);
		void handleBinaryData(TransmitContext ctx, byte[] data);
	}
}
