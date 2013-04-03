using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	class TransmitContext
	{
		private IStateTransmit state;

		public string file_name { get; set; }
		public long file_size { get; set; }
		public ITempFile temp_file { get; set; }

		public IFileStorage storage { get; private set; }
		public ITempFileFactory factory { get; private set; }

		public TransmitContext(ITempFileFactory factory, IFileStorage storage)
		{
			this.factory = factory;
			this.storage = storage;

			this.state = new TransmitInitState();
		}

		public void SetState(IStateTransmit newState)
		{
			state = newState;
		}

		public void handleFileStartCmd(TextCommand cmd)
		{
			state.handleFileStartCmd(this, cmd);
		}

		public void handleFileEndCmd(TextCommand cmd)
		{
			state.handleFileEndCmd(this, cmd);
		}

		public void handleBinaryData(byte[] data)
		{
			state.handleBinaryData(this, data);
		}
	}
}
