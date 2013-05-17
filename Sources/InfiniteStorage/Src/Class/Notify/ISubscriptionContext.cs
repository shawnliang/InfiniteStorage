
namespace InfiniteStorage.Notify
{
	public interface ISubscriptionContext
	{
		string device_name { get; }

		string device_id { get; }

		long files_from_seq { get; }

		bool subscribe_files { get; }

		bool subscribe_labels { get; }

		void Send(string data);
	}
}
