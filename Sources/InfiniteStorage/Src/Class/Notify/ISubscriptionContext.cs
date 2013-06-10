
namespace InfiniteStorage.Notify
{
	public interface ISubscriptionContext
	{
		string device_name { get; }

		string device_id { get; }

		long files_from_seq { get; }

		long labels_from_seq { get; set; }

		bool subscribe_files { get; }

		bool subscribe_labels { get; }

		bool subscribe_devices { get; }

		void Send(string data);
	}
}
