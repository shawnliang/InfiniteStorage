using System;

namespace Waveface.Client
{
	public class UnSortedItemEventArgs : EventArgs
	{
		public string DeviceID { get; private set; }

		public UnSortedItemEventArgs(string deviceID)
		{
			this.DeviceID = deviceID;
		}
	}
}
