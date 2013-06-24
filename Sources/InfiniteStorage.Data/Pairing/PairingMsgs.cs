using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Data.Pairing
{
	public class PairingClientMsgs
	{
		public subscribe subscribe { get; set; }

		public pairing_mode pairing_mode { get; set; }

		public accept accept { get; set; }
	}

	public class PairingServerMsgs
	{
		public pairing_request pairing_request { get; set; }
	}

	public class subscribe
	{
		public bool pairing { get; set; }
	}

	public class pairing_mode
	{
		public bool enabled { get; set; }
	}

	public class accept
	{
		public string device_id { get; set; }
	}

	public class pairing_request
	{
		public string device_id { get; set; }
		public string device_name { get; set; }
	}
}
