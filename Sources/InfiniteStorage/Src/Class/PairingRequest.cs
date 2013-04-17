using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage
{
	public class PairingRequestEventArgs : EventArgs
	{
		public WebsocketProtocol.ProtocolContext Context;
		private Action approveDelegate;

		public PairingRequestEventArgs(WebsocketProtocol.ProtocolContext ctx)
		{
		}

		public void Approve()
		{
			try
			{
				Context.handleApprove();
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger(typeof(PairingRequestEventArgs)).Warn("Uable to approve request for " + Context.device_name, e);
			}
		}
	}
}
