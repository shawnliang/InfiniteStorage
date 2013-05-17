using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InfiniteStorage.Model;

namespace InfiniteStorage.WebsocketProtocol
{
	public class TransmitStartedState : AbstractProtocolState
	{
		public ITransmitStateUtility Util { get; set; }

		public TransmitStartedState()
		{
			Util = new TransmitUtility();
		}

		public override void handleBinaryData(ProtocolContext ctx, byte[] data)
		{
			ctx.temp_file.Write(data);
			log4net.LogManager.GetLogger("wsproto").DebugFormat("file content of {0}: {1} bytes", ctx.fileCtx.file_name, data.Length);
		}

		public override void handleFileEndCmd(ProtocolContext ctx, TextCommand cmd)
		{
			ctx.temp_file.EndWrite();

			if (!Util.HasDuplicateFile(ctx.fileCtx, ctx.device_id))
			{
				SavedPath saved = null;
				try
				{
					saved = ctx.storage.MoveToStorage(ctx.temp_file.Path, ctx.fileCtx);
				}
				catch (Exception e)
				{
					throw new IOException("Unable to move temp file to storage. temp_file:" + ctx.temp_file.Path + ", file_name: " + ctx.fileCtx.file_name, e);
				}

				var fileAsset = new FileAsset
				{
					device_id = ctx.device_id,
					event_time = ctx.fileCtx.datetime,
					file_id = Guid.NewGuid(),
					file_name = ctx.fileCtx.file_name,
					file_path = Path.Combine(ctx.fileCtx.folder, ctx.fileCtx.file_name),
					file_size = ctx.fileCtx.file_size,
					type = (int)ctx.fileCtx.type,
					saved_path = saved.relative_file_path,
					parent_folder = Path.GetDirectoryName(saved.relative_file_path),
					seq = Util.GetNextSeq()
				};
				Util.SaveFileRecord(fileAsset);


				if (ctx.fileCtx.file_size != ctx.temp_file.BytesWritten)
					log4net.LogManager.GetLogger(typeof(TransmitStartedState)).WarnFormat("{0} is expected to have {1} bytes but {2} bytes received.", ctx.fileCtx.file_name, ctx.fileCtx.file_size, ctx.temp_file.BytesWritten);

				log4net.LogManager.GetLogger("wsproto").Debug("file-end: " + ctx.fileCtx.file_name);
			}
			else
			{
				ctx.temp_file.Delete();
			}

			

			ctx.recved_files++;
			ctx.SetState(new TransmitInitState());
		}
	}
}
