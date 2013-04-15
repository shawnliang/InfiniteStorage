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
			throw new ProtocolErrorException(unexpectedCmd(cmd));
		}

		public virtual void handleFileEndCmd(ProtocolContext ctx, TextCommand cmd)
		{
			throw new ProtocolErrorException(unexpectedCmd(cmd));
		}

		public virtual void handleBinaryData(ProtocolContext ctx, byte[] data)
		{
			throw new ProtocolErrorException("binary data is not expected in this state: " + ToString());
		}

		public virtual void handleConnectCmd(ProtocolContext ctx, TextCommand cmd)
		{
			throw new ProtocolErrorException(unexpectedCmd(cmd));
		}

		private string unexpectedCmd(TextCommand cmd)
		{
			return string.Format("{0} cmd is not expected in {1}", cmd.action, this.ToString());
		}
	}
}
