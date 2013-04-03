using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class ProtocolErrorException : ApplicationException
	{
		public ProtocolErrorException()
		{
		}

		public ProtocolErrorException(string message)
			:base(message)
		{
		}

		public ProtocolErrorException(string message, Exception innerException)
			:base(message, innerException)
		{
		}
	}
}
