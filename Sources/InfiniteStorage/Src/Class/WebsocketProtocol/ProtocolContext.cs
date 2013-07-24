using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace InfiniteStorage.WebsocketProtocol
{
	public delegate void SendTextDelegate(string data);

	public delegate void StopDelegate(WebSocketSharp.Frame.CloseStatusCode code, string reason);
	public class ProtocolContext : IProtocolHandlerContext, IConnectionStatus
	{
		private AbstractProtocolState state;
		private Dictionary<string, object> memo = new Dictionary<string, object>();

		public FileContext fileCtx { get; set; }
		public ITempFile temp_file { get; set; }
		public string device_name { get; set; }
		public string device_id { get; set; }
		public string device_folder_name { get; set; }

		public long backup_count { get; set; }
		public long total_count { get; set; }
		public long recved_files { get; set; }

		public string passcode { get; set; }

		public IFileStorage storage { get; private set; }
		public ITempFileFactory factory { get; private set; }

		public SendTextDelegate SendFunc { private get; set; }
		public StopDelegate StopFunc { private get; set; }
		public Func<bool> PingFunc { private get; set; }

		public event EventHandler<WebsocketEventArgs> OnConnectAccepted;
		public event EventHandler<WebsocketEventArgs> OnPairingRequired;
		public event EventHandler<WebsocketEventArgs> OnTotalCountUpdated;
		public event EventHandler<WebsocketEventArgs> OnFileReceiving;
		public event EventHandler<WebsocketEventArgs> OnFileEnding;
		public event EventHandler<WebsocketEventArgs> OnFileReceived;
		public event EventHandler<ThumbnailReceivedEventArgs> OnThumbnailReceived;

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

		public void handleUpdateCountCmd(TextCommand cmd)
		{
			state.handleUpdateCountCmd(this, cmd);
		}

		public void handleThumbStartCmd(TextCommand cmd)
		{
			state.handleThumbStartCmd(this, cmd);
		}

		public void handleThumbEndCmd(TextCommand cmd)
		{
			state.handleThumbEndCmd(this, cmd);
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
			log4net.LogManager.GetLogger("pairing").Debug("Clear context");
			SetState(new UnconnectedState());

			IsClosed = true;

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

		public void raiseOnTotalCountUpdated()
		{
			var handler = OnTotalCountUpdated;
			if (handler != null)
			{
				handler(this, new WebsocketEventArgs(this));
			}
		}

		internal void raiseOnFileReceived()
		{
			var handler = OnFileReceived;
			if (handler != null)
			{
				handler(this, new WebsocketEventArgs(this));
			}
		}
		
		public void raiseOnThumbnailReceived(string thumbPath, int transferCount)
		{
			var handler = OnThumbnailReceived;
			if (handler != null)
				handler(this, new ThumbnailReceivedEventArgs(thumbPath, this.device_id, transferCount));
		}

		public void raiseOnFileEnding()
		{
			var handler = OnFileEnding;
			if (handler != null)
				handler(this, new WebsocketEventArgs(this));
		}

		public void raiseOnFileReceiving()
		{
			var handler = OnFileReceiving;
			if (handler != null)
				handler(this, new WebsocketEventArgs(this));

		}

		public void Send(object data)
		{
			var msg = data as string;
			if (msg == null)
				msg = JsonConvert.SerializeObject(data, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc, NullValueHandling = NullValueHandling.Ignore, DateFormatHandling = DateFormatHandling.IsoDateFormat });

			SendFunc(msg);
		}

		public void Stop(WebSocketSharp.Frame.CloseStatusCode code, string reason)
		{
			StopFunc(code, reason);
		}

		public bool IsClosed { get; set; }

		public bool NoMoreToTransfer()
		{
			return total_count > 0 && total_count == backup_count;
		}


		public bool IsRecving
		{
			get { return total_count > 0 && total_count != backup_count; }
		}


		public bool Ping()
		{
			var func = PingFunc;

			if (func != null)
				return func();

			else
				throw new InvalidOperationException("PingFunc is not set");
		}

		public bool ContainsData(string key)
		{
			return memo.ContainsKey(key);
		}

		public void SetData(string key, object data)
		{
			if (memo.ContainsKey(key))
				memo.Remove(key);

			memo.Add(key, data);
		}

		public object GetData(string key)
		{
			return memo[key];
		}
	}
}
