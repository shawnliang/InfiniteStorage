using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	class TransmitStartedState : IStateTransmit
	{
		public void handleBinaryData(TransmitContext ctx, byte[] data)
		{
			ctx.temp_file.Write(data);
		}

		public void handleFileStartCmd(TransmitContext ctx, TextCommand cmd)
		{
			throw new ProtocolErrorException("file-start cmd is not expected in transmit-started state");
		}

		public void handleFileEndCmd(TransmitContext ctx, TextCommand cmd)
		{
			ctx.temp_file.EndWrite();
			ctx.storage.MoveToStorage(ctx.temp_file.Path, ctx.file_name);

			ctx.SetState(new TransmitInitState());
		}
	}
}
