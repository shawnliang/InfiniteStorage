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

			if (ctx.file_size != ctx.temp_file.BytesWritten)
				log4net.LogManager.GetLogger("").WarnFormat("{0} is expected to have {1} bytes but {2} bytes received.", ctx.file_name, ctx.file_size, ctx.temp_file.BytesWritten);

			ctx.SetState(new TransmitInitState());
		}
	}
}
