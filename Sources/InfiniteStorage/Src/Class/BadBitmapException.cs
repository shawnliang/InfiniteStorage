using System;

namespace InfiniteStorage
{
	class BadBitmapException : Exception
	{
		public BadBitmapException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}
}
