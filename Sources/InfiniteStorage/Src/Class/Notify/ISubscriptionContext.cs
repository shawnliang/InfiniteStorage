using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Notify
{
	public interface ISubscriptionContext
	{
		string device_name { get; }
		
		string device_id { get; }

		long files_from_seq { get; }
		
		void Send(string data);
	}
}
