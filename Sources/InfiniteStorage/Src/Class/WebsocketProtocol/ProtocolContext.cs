using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace InfiniteStorage.WebsocketProtocol
{
	public delegate void SendTextDelegate(string data);

	public delegate void StopDelegate(WebSocketSharp.Frame.CloseStatusCode code, string reason);
	public class ProtocolContext : IProtocolHandlerContext, IConnectionStatus
	{
		private AbstractProtocolState state;
		public FileContext fileCtx { get; set; }
		public ITempFile temp_file { get; set; }
		public string device_name { get; set; }
		public string device_id { get; set; }
		public string device_folder_name { get; set; }

		public long total_files { get; set; }
		public long recved_files { get; set; }

		public IFileStorage storage { get; private set; }
		public ITempFileFactory factory { get; private set; }

		public SendTextDelegate SendText { private get; set; }
		public StopDelegate Stop { get; set; }

		public event EventHandler<WebsocketEventArgs> OnConnectAccepted;
		public event EventHandler<WebsocketEventArgs> OnPairingRequired;

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

		public void handleApprove()
		{
			state.handleApprove(this);
		}

		public void handleDisapprove()
		{
			state.handleDisapprove(this);
		}

		public void Clear()
		{
			if (temp_file != null)
			{
				temp_file.Delete();
				temp_file = null;
			}
		}

		public void raiseOnConnectAccepted()
		{
			var handler = OnConnectAccepted;
			if (handler != null)
			{
				handler(this, new WebsocketEventArgs(this));
			}
		}

		public void raiseOnPairingRequired()
		{
			var handler = OnPairingRequired;
			if (handler != null)
			{
				handler(this, new WebsocketEventArgs(this));
			}
		}

		public void Send(object data)
		{
			if (data is string)
			{
				SendText((string)data);
			}
			else
			{
				var txt = JsonConvert.SerializeObject(data, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc, NullValueHandling = NullValueHandling.Ignore, DateFormatHandling = DateFormatHandling.IsoDateFormat });
				SendText(txt);
			}
		}
	}
}
