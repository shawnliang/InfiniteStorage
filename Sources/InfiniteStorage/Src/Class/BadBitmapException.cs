#region

using System;

#endregion

namespace InfiniteStorage
{
	internal class BadBitmapException : Exception
	{
		public BadBitmapException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}
}