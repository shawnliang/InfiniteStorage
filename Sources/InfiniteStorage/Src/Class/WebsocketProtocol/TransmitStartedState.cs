using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

			try
			{
				ctx.storage.MoveToStorage(ctx.temp_file.Path, ctx.file_name);
			}
			catch (Exception e)
			{
				throw new IOException("Unable to move temp file to storage. temp_file:" + ctx.temp_file.Path + ", file_name: " + ctx.file_name, e);
			}

			if (ctx.file_size != ctx.temp_file.BytesWritten)
				log4net.LogManager.GetLogger(typeof(TransmitStartedState)).WarnFormat("{0} is expected to have {1} bytes but {2} bytes received.", ctx.file_name, ctx.file_size, ctx.temp_file.BytesWritten);

			ctx.SetState(new TransmitInitState());
		}
	}
}
