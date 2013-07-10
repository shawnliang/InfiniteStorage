
namespace InfiniteStorage
{
	public interface IConnectionStatus
	{
		string device_name { get; }

		string device_id { get; }

		void Send(object data);

		void Stop(WebSocketSharp.Frame.CloseStatusCode code, string reason);

		bool IsRecving { get; }

		bool Ping();
	}
}
