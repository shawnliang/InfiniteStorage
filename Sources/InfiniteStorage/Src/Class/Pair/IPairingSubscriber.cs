
using InfiniteStorage.Data.Pairing;
using Newtonsoft.Json;

namespace InfiniteStorage.Pair
{
	public interface IPairingSubscriber
	{
		void NewPairingRequestComing(string device_id, string device_name);
		void ThumbnailReceived(string path, int transferCount);
	}

	class PairingSubscriber : IPairingSubscriber
	{
		private PairWebSocketService peer;

		public PairingSubscriber(PairWebSocketService peer)
		{
			this.peer = peer;
		}

		public void NewPairingRequestComing(string device_id, string device_name)
		{
			var msg = JsonConvert.SerializeObject(
				new PairingServerMsgs
				{
					pairing_request = new pairing_request
					{
						device_id = device_id,
						device_name = device_name
					}
				});

			log4net.LogManager.GetLogger("pairing").Debug("send pairing request to UI");

			peer.Send(msg);
		}


		public void ThumbnailReceived(string path, int transferCount)
		{
			var msg = JsonConvert.SerializeObject(
				new PairingServerMsgs
				{
					thumb_received = new thumb_info
					{
						path = path,
						transfer_count = transferCount
					}
				});

			log4net.LogManager.GetLogger("pairing").Debug("send thumbnail to UI");

			peer.Send(msg);
		}
	}
}