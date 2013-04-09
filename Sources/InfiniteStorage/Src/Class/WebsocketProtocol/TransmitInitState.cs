using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	class TransmitInitState : IStateTransmit
	{
		public void handleBinaryData(TransmitContext ctx, byte[] data)
		{
			throw new ProtocolErrorException("binary data is not expected in transmit-init state");
		}


		public void handleFileStartCmd(TransmitContext ctx, TextCommand cmd)
		{
			ctx.file_name = cmd.file_name;
			ctx.file_size = cmd.file_size;
			ctx.temp_file = ctx.factory.CreateTempFile();

			log4net.LogManager.GetLogger("wsproto").Debug("file-start: " + ctx.file_name);
			ctx.SetState(new TransmitStartedState());
		}

		public void handleFileEndCmd(TransmitContext ctx, TextCommand cmd)
		{
			throw new ProtocolErrorException("file-end cmd is not expected in transmit-init state");
		}
	}
}
