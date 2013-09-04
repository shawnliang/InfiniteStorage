using System;

namespace Waveface.Client
{
	public class UnSortedItemEventArgs : EventArgs
	{
		public String DeviceID { get; private set; }

		public UnSortedItemEventArgs(String deviceID)
		{
			this.DeviceID = deviceID;
		}
	}
}
