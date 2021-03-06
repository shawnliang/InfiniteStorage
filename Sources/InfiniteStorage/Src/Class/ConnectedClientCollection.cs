﻿using System.Collections.Generic;
using System.Linq;

namespace InfiniteStorage
{
	class ConnectedClientCollection
	{
		private static ConnectedClientCollection instance = new ConnectedClientCollection();

		private object cs = new object();
		private LinkedList<IConnectionStatus> connections = new LinkedList<IConnectionStatus>();

		private ConnectedClientCollection()
		{
		}

		public void Add(IConnectionStatus status)
		{
			lock (cs)
			{
				if (!connections.Contains(status))
					connections.AddLast(status);
			}
		}

		public void Remove(IConnectionStatus status)
		{
			lock (cs)
			{
				connections.Remove(status);
			}
		}

		public List<IConnectionStatus> GetAllConnections()
		{
			lock (cs)
			{
				return connections.ToList();
			}
		}


		public static ConnectedClientCollection Instance
		{
			get { return instance; }
		}
	}
}
