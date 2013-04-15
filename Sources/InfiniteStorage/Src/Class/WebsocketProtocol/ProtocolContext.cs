using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public class ProtocolContext
	{
		private IProtocolState state;
		public FileContext fileCtx { get; set; }
		public ITempFile temp_file { get; set; }

		public IFileStorage storage { get; private set; }
		public ITempFileFactory factory { get; private set; }

		public ProtocolContext(ITempFileFactory factory, IFileStorage storage, IProtocolState initialState)
		{
			this.factory = factory;
			this.storage = storage;

			this.state = initialState;
		}

		public void SetState(IProtocolState newState)
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
