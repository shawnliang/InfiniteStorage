namespace InfiniteStorage.Data.Notify
{
	public class SubscribeMsg
	{
		public connect connect { get; set; }
		public subscribe subscribe { get; set; }
	}

	public class connect
	{
		public string device_id { get; set; }
		public string device_name { get; set; }
	}

	public class subscribe
	{
		public long? files_from_seq { get; set; }
		public bool labels { get; set; }
		public long? labels_from_seq { get; set; }
		public bool devices { get; set; }

		public bool ui_change { get; set; }
	}
}