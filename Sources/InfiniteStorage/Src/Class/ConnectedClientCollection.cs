using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage
{
	class ConnectedClientCollection
	{
		private static ConnectedClientCollection instance;
		
		private object cs = new object();
		private LinkedList<IConnectionStatus> connections = new LinkedList<IConnectionStatus>();
		private LinkedList<PairingRequestEventArgs> requests = new LinkedList<PairingRequestEventArgs>();

		private ConnectedClientCollection()
		{
		}

		static ConnectedClientCollection()
		{
			instance = new ConnectedClientCollection();
		}

		public void Add(IConnectionStatus status)
		{
			lock (cs)
			{
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

		public void AddPairingRequest(PairingRequestEventArgs pairingRequest)
		{
			lock (cs)
			{
				requests.AddLast(pairingRequest);
			}
		}

		public void RemovePairingRequest(PairingRequestEventArgs pairingRequest)
		{
			lock (cs)
			{
				requests.Remove(pairingRequest);
			}
		}

		public List<PairingRequestEventArgs> GetAllPairingRequests()
		{
			lock (cs)
			{
				return requests.ToList();
			}
		}

		public static ConnectedClientCollection Instance
		{
			get { return instance; }
		}

		
	}
}
