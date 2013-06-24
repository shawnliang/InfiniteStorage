
using InfiniteStorage.Data.Pairing;
using Newtonsoft.Json;

namespace InfiniteStorage.Pair
{
	public interface IPairingSubscriber
	{
		void NewPairingRequestComing(string device_id, string device_name);
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

			peer.Send(msg);
		}
	}
}