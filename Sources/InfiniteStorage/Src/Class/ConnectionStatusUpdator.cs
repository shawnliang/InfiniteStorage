#region

using System;
using System.Linq;
using InfiniteStorage.WebsocketProtocol;
using log4net;

#endregion

namespace InfiniteStorage
{
	internal class ConnectionStatusUpdator
	{
		public void UpdateStatusToPeers()
		{
			var peers = ConnectedClientCollection.Instance.GetAllConnections();
			var util = new ConnectMsgHandlerUtil();

			foreach (var peer in peers.Distinct())
			{
				try
				{
					peer.Ping();
					LogManager.GetLogger(GetType()).Debug("Ping to " + peer.device_name);
				}
				catch (Exception e)
				{
					LogManager.GetLogger(GetType()).Warn("Unable to send backup info to peer:" + peer.device_name, e);
				}
			}
		}
	}
}