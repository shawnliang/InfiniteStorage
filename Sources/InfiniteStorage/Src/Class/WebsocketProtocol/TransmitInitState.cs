using InfiniteStorage.Model;
using System;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface IFileUtility
	{
		bool HasDuplicateFile(FileContext file, string device_id);
	}


	public class TransmitInitState : AbstractProtocolState
	{
		private IFileUtility util;

		public TransmitInitState()
		{
			this.util = new TransmitUtility();
		}

		public TransmitInitState(IFileUtility util)
		{
			this.util = util;
		}

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

			ctx.backup_count = cmd.backuped_count;
			ctx.total_count = cmd.total_count;

			var fileCtx = new FileContext
			{
				file_name = cmd.file_name,
				file_size = cmd.file_size,
				folder = cmd.folder,

				is_thumbnail = cmd.is_thumbnail,
				datetime = cmd.datetime,
				type = type
			};

			if (!string.IsNullOrEmpty(cmd.object_id))
				fileCtx.file_id = new Guid(cmd.object_id);

			if (!fileCtx.is_thumbnail && util.HasDuplicateFile(fileCtx, ctx.device_id))
			{
				ctx.fileCtx = null;
				ctx.raiseOnFileReceiving();
				ctx.raiseOnFileDropped();

				ctx.Send(new TextCommand { action = "file-exist", file_name = cmd.file_name });
				log4net.LogManager.GetLogger("wsproto").Debug("file duplicate! send back file-exist");
			}
			else
			{
				ctx.fileCtx = fileCtx;
				ctx.temp_file = ctx.factory.CreateTempFile();
				ctx.raiseOnFileReceiving();

				if (!fileCtx.is_thumbnail)
					ctx.IsPreparing = false;

				if (!fileCtx.is_thumbnail)
				{
					ctx.Send(new TextCommand { action = "file-go", file_name = cmd.file_name });
				}
				ctx.SetState(new TransmitStartedState());
			}
		}

		public override void handleUpdateCountCmd(ProtocolContext ctx, TextCommand cmd)
		{
			ctx.total_count = cmd.transfer_count;
			ctx.backup_count = cmd.backuped_count;
			ctx.raiseOnTotalCountUpdated();
		}

		public override void handleThumbStartCmd(ProtocolContext protocolContext, TextCommand cmd)
		{
		}

		public override void handleThumbEndCmd(ProtocolContext protocolContext, TextCommand cmd)
		{
		}

		public override void handleBinaryData(ProtocolContext ctx, byte[] data)
		{
		}
	}
}
