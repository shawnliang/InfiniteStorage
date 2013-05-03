using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;
using Newtonsoft.Json;
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

		public override void handleApprove(ProtocolContext ctx)
		{
			var dev = new Device
			{
				device_id = ctx.device_id,
				device_name = ctx.device_name,
				folder_name = Util.GetUniqueDeviceFolder(ctx.device_name)
			};

			Util.Save(dev);
			ctx.device_folder_name = dev.folder_name;
			approveHandler.ReplyAcceptMsgToDevice(ctx, dev);
		}

		public override void handleDisapprove(ProtocolContext ctx)
		{
			var response = new
			{
				action = "denied",
				reason = "user rejected"
			};

			ctx.Send(response);
			ctx.Stop(WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, "User rejected");
			ctx.SetState(new UnconnectedState());
		}
	}
}
