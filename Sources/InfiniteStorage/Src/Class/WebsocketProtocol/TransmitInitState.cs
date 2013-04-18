using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class TransmitInitState : AbstractProtocolState
	{
		public override void handleFileStartCmd(ProtocolContext ctx, TextCommand cmd)
		{
			ctx.fileCtx = new FileContext
			{
				file_name = cmd.file_name,
				file_size = cmd.file_size,
				folder = cmd.folder,
				mimetype = cmd.mimetype,
				datetime = cmd.datetime,
				UTI = cmd.UTI
			};

			ctx.temp_file = ctx.factory.CreateTempFile();

			log4net.LogManager.GetLogger("wsproto").DebugFormat("name: {0}, size: {1}, folder: {2}, mime: {3}, datetime: {4}, uti: {5}",
				cmd.file_name, cmd.file_size, cmd.folder, cmd.mimetype, cmd.datetime, cmd.UTI);

			ctx.SetState(new TransmitStartedState());
		}
	}
}
