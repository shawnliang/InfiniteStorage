using System;

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
