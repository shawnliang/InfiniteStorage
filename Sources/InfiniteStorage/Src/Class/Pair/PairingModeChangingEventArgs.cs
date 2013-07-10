using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Pair
{
	class PairingModeChangingEventArgs : EventArgs
	{
		public PairingModeChangingEventArgs(bool enable)
		{
			enabled = enable;
		}

		public bool enabled { get; set; }
	}
}
