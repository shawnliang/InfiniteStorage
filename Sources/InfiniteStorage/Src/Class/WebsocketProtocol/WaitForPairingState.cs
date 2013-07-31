using InfiniteStorage.Model;
namespace InfiniteStorage.WebsocketProtocol
{
	public class WaitForApproveState : AbstractProtocolState
	{
		public IConnectMsgHandlerUtil Util
		{
			get { return approveHandler.Util; }
			set { approveHandler.Util = value; }
		}

		private ConnectMsgHandler approveHandler;

		public WaitForApproveState()
		{
			this.approveHandler = new ConnectMsgHandler();
		}

		public override void handleApprove(ProtocolContext ctx, int latest_x_items = int.MaxValue)
		{
			var dev = new Device
			{
				device_id = ctx.device_id,
				device_name = ctx.device_name,
				folder_name = Util.GetUniqueDeviceFolder(ctx.device_name),
				sync_init_count = latest_x_items
			};

			Util.Save(dev);
			ctx.device_folder_name = dev.folder_name;
			approveHandler.ReplyAcceptMsgToDevice(ctx, dev, latest_x_items);
		}

		public override void handleDisapprove(ProtocolContext ctx)
		{
			var response = new
			{
				action = "denied",
				reason = "user rejected"
			};

			log4net.LogManager.GetLogger("pairing").Debug("send denied");
			ctx.Send(response);
			ctx.Stop(WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, "User rejected");
			ctx.SetState(new UnconnectedState());
		}

		public override void handleThumbStartCmd(ProtocolContext ctx, TextCommand cmd)
		{
			ctx.SetData(ThumbnailStartedState.TEMP_FILE_KEY, ctx.factory.CreateTempFile());
			ctx.SetState(new ThumbnailStartedState());
		}
	}
}
