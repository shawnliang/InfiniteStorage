using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;

namespace InfiniteStorage.WebsocketProtocol
{
	public class TransmitInitState : AbstractProtocolState
	{
		public override void handleFileStartCmd(ProtocolContext ctx, TextCommand cmd)
		{
			if (string.IsNullOrEmpty(cmd.type))
				throw new ProtocolErrorException("missing fied: type");
			if (string.IsNullOrEmpty(cmd.file_name))
				throw new ProtocolErrorException("missing fied: file_name");
			if (string.IsNullOrEmpty(cmd.folder))
				throw new ProtocolErrorException("missing fied: folder");

			FileAssetType type;
			if (!Enum.TryParse<FileAssetType>(cmd.type, true, out type))
				throw new ProtocolErrorException("unknown type: " + cmd.type);

			ctx.fileCtx = new FileContext
			{
				file_name = cmd.file_name,
				file_size = cmd.file_size,
				folder = cmd.folder,

				datetime = cmd.datetime,
				type = type
			};

			ctx.backup_count = cmd.backuped_count;
			ctx.total_count = cmd.total_count;

			ctx.temp_file = ctx.factory.CreateTempFile();

			log4net.LogManager.GetLogger("wsproto").DebugFormat("name: {0}, size: {1}, folder: {2}, datetime: {3}, type: {4}",
				cmd.file_name, cmd.file_size, cmd.folder, cmd.datetime, cmd.type);

			ctx.SetState(new TransmitStartedState());
		}

		public override void handleUpdateCountCmd(ProtocolContext ctx, TextCommand cmd)
		{
			ctx.total_count = cmd.total_count;
			ctx.backup_count = cmd.backuped_count;
			ctx.raiseOnTotalCountUpdated();
		}
	}
}
