using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Client
{
	public class UnSortedItemEventArgs:EventArgs
	{
		public string DeviceID { get; private set; }

		public UnSortedItemEventArgs(string deviceID)
		{
			this.DeviceID = deviceID;
		}
	}
}
