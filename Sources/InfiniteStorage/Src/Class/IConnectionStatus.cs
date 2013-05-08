using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage
{
	public interface IConnectionStatus
	{
		string device_name { get; }

		string device_id { get; }

		void Send(object data);

		void Stop(WebSocketSharp.Frame.CloseStatusCode code, string reason);
	}
}
