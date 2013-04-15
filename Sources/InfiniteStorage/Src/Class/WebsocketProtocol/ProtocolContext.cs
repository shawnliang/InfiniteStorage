using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public delegate void SendTextDelegate(string data);

	public class ProtocolContext : IProtocolHandlerContext
	{
		private AbstractProtocolState state;
		public FileContext fileCtx { get; set; }
		public ITempFile temp_file { get; set; }
		public string device_name { get; set; }
		public string device_id { get; set; }

		public IFileStorage storage { get; private set; }
		public ITempFileFactory factory { get; private set; }

		public SendTextDelegate SendText { get; set; }

		public ProtocolContext(ITempFileFactory factory, IFileStorage storage, AbstractProtocolState initialState)
		{
			this.factory = factory;
			this.storage = storage;

			this.state = initialState;
		}

		public void SetState(AbstractProtocolState newState)
		{
			state = newState;
		}

		public AbstractProtocolState GetState()
		{
			return state;
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

		public void handleConnectCmd(TextCommand cmd)
		{
			state.handleConnectCmd(this, cmd);
		}

		public void Clear()
		{
			if (temp_file != null)
			{
				temp_file.Delete();
				temp_file = null;
			}
		}
	}
}
