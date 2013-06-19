using InfiniteStorage.Properties;
using System;
using System.IO;
using System.Linq;

namespace InfiniteStorage
{
	class ConnectionStatusUpdator
	{
		public void UpdateStatusToPeers()
		{
			var peers = ConnectedClientCollection.Instance.GetAllConnections();
			var util = new WebsocketProtocol.ConnectMsgHandlerUtil();

			foreach (var peer in peers.Distinct())
			{
				try
				{
					peer.Ping();
					log4net.LogManager.GetLogger(GetType()).Debug("Ping to " + peer.device_name);
				}
				catch (Exception e)
				{
					log4net.LogManager.GetLogger(this.GetType()).Warn("Unable to send backup info to peer:" + peer.device_name, e);
				}
			}
		}
	}
}
