using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public abstract class AbstractProtocolState
	{
		public virtual void handleFileStartCmd(ProtocolContext ctx, TextCommand cmd)
		{
			throw new ProtocolErrorException(errorMsg(cmd.action));
		}

		public virtual void handleFileEndCmd(ProtocolContext ctx, TextCommand cmd)
		{
			throw new ProtocolErrorException(errorMsg(cmd.action));
		}

		public virtual void handleBinaryData(ProtocolContext ctx, byte[] data)
		{
			throw new ProtocolErrorException(errorMsg("binary data"));
		}

		public virtual void handleConnectCmd(ProtocolContext ctx, TextCommand cmd)
		{
			throw new ProtocolErrorException(errorMsg(cmd.action));
		}

		private string errorMsg(string somethingUnexepected)
		{
			return string.Format("{0} is not expected in {1}", somethingUnexepected, this.ToString());
		}

		public virtual void handleApprove(ProtocolContext protocolContext)
		{
			throw new ProtocolErrorException(errorMsg("approve"));
		}

		public virtual void handleDisapprove(ProtocolContext protocolContext)
		{
			throw new ProtocolErrorException(errorMsg("disapprove"));
		}
	}
}
