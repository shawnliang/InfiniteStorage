

namespace InfiniteStorage.Data.Pairing
{
	public class PairingClientMsgs
	{
		public subscribe subscribe { get; set; }

		public pairing_mode pairing_mode { get; set; }

		public accept_reject accept { get; set; }

		public accept_reject reject { get; set; }
	}

	public class PairingServerMsgs
	{
		public pairing_request pairing_request { get; set; }
		public thumb_info thumb_received { get; set; }
	}

	public class subscribe
	{
		public bool pairing { get; set; }
	}

	public class pairing_mode
	{
		public bool enabled { get; set; }
	}

	public class accept_reject
	{
		public string device_id { get; set; }
		public bool sync_old { get; set; }
		public int last_x_days { get; set; }
	}

	public class pairing_request
	{
		public string request_id { get; set; }
		public string device_id { get; set; }
		public string device_name { get; set; }
	}

	public class thumb_info
	{
		public int transfer_count { get; set; }
		public string path { get; set; }
		public string request_id { get; set; }
	}
}