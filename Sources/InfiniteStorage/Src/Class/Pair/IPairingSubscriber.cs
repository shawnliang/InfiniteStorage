
using InfiniteStorage.Data.Pairing;
using Newtonsoft.Json;

namespace InfiniteStorage.Pair
{
	public interface IPairingSubscriber
	{
		void NewPairingRequestComing(string device_id, string device_name, string requestId);
		void ThumbnailReceived(string path, int transferCount, string requestId);
	}

	class PairingSubscriber : IPairingSubscriber
	{
		private PairWebSocketService peer;

		public PairingSubscriber(PairWebSocketService peer)
		{
			this.peer = peer;
		}

		public void NewPairingRequestComing(string device_id, string device_name, string requestId)
		{
			var msg = JsonConvert.SerializeObject(
				new PairingServerMsgs
				{
					pairing_request = new pairing_request
					{
						request_id = requestId,
						device_id = device_id,
						device_name = device_name
					}
				});

			log4net.LogManager.GetLogger("pairing").Debug("send pairing request to UI");

			peer.Send(msg);
		}


		public void ThumbnailReceived(string path, int transferCount, string requestId)
		{
			var msg = JsonConvert.SerializeObject(
				new PairingServerMsgs
				{
					thumb_received = new thumb_info
					{
						request_id = requestId,
						path = path,
						transfer_count = transferCount
					}
				});

			log4net.LogManager.GetLogger("pairing").Debug("send thumbnail to UI");

			peer.Send(msg);
		}
	}
}