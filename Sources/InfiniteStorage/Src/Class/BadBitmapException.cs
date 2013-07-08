using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage
{
	class BadBitmapException : Exception
	{
		public BadBitmapException(string msg, Exception innerException)
			:base(msg, innerException)
		{
		}
	}
}
