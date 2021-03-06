﻿
namespace InfiniteStorage.WebsocketProtocol
{
	public class UnconnectedState : AbstractProtocolState
	{
		public IConnectMsgHandler handler { get; set; }

		public UnconnectedState()
		{
			this.handler = new ConnectMsgHandler();
		}

		public override void handleConnectCmd(ProtocolContext ctx, TextCommand cmd)
		{
			ctx.device_id = cmd.device_id;
			ctx.device_name = cmd.device_name;

			handler.HandleConnectMsg(cmd, ctx);
		}
	}
}
