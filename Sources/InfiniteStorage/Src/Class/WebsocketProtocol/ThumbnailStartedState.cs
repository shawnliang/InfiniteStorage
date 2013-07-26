using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class ThumbnailStartedState : AbstractProtocolState
	{
		public const string TEMP_FILE_KEY = "thumbTemp";

		public override void handleBinaryData(ProtocolContext ctx, byte[] data)
		{
			var temp = ctx.GetData(TEMP_FILE_KEY) as ITempFile;
			temp.Write(data);
		}

		public override void handleThumbEndCmd(ProtocolContext ctx, TextCommand cmd)
		{
			var temp = ctx.GetData(TEMP_FILE_KEY) as ITempFile;
			temp.EndWrite();

			ctx.raiseOnThumbnailReceived(temp.Path, (int)cmd.transfer_count);

			ctx.SetState(new WaitForApproveState());
		}

		public override void handleApprove(ProtocolContext ctx, bool syncOld, int latest_x_items)
		{
			var impl = new WaitForApproveState();
			impl.handleApprove(ctx, syncOld, latest_x_items);
		}
	}
}
