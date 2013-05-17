using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfiniteStorage.Notify
{
	public class NotifyChannelHandler
	{
		private const string PROPERTY_CONNECT = "connect";
		private const string PROPERTY_DEVICE_ID = "device_id";
		private const string PROPERTY_DEVICE_NAME = "device_name";
		private const string PROPERTY_SUBSCRIBE = "subscribe";
		private const string PROPERTY_FILES_FROM_SEQ = "files_from_seq";
		private const string PROPERTY_LABELS = "labels";
		private const string PROPERTY_LABELS_FROM_SEQ = "labels_from_seq";

		public event EventHandler<NotifyChannelEventArgs> Subscribing;

		public SubscriptionContext Ctx { get; set; }


		public void HandleMessage(WebSocketService svc, MessageEventArgs args)
		{
			var msg = JObject.Parse(args.Data);

			if (Ctx != null)
				throw new WebsocketProtocol.ProtocolErrorException("already connected");

			if (msg[PROPERTY_CONNECT] == null)
				throw new WebsocketProtocol.ProtocolErrorException("connect is expected");

			if (msg[PROPERTY_SUBSCRIBE] == null)
				throw new WebsocketProtocol.ProtocolErrorException("subscribe is expected");

			Ctx = new SubscriptionContext(
				msg[PROPERTY_CONNECT][PROPERTY_DEVICE_ID].Value<string>(),
				msg[PROPERTY_CONNECT][PROPERTY_DEVICE_NAME].Value<string>(),
				svc);

			var subsribe = msg[PROPERTY_SUBSCRIBE];
			if (subsribe[PROPERTY_FILES_FROM_SEQ] != null)
			{
				Ctx.subscribe_files = true;
				Ctx.files_from_seq = subsribe[PROPERTY_FILES_FROM_SEQ].Value<long>();
			}

			if (subsribe[PROPERTY_LABELS] != null)
			{
				Ctx.subscribe_labels = subsribe[PROPERTY_LABELS].Value<bool>();
				Ctx.labels_from_seq = (subsribe[PROPERTY_LABELS_FROM_SEQ] != null) ? subsribe[PROPERTY_LABELS_FROM_SEQ].Value<long>() : 0;
			}

			raiseSubscribingEvent(svc, Ctx);
		}

		private void raiseSubscribingEvent(WebSocketService svc, SubscriptionContext Ctx)
		{
			var handler = Subscribing;
			if (handler != null)
				handler(svc, new NotifyChannelEventArgs { Ctx = Ctx });
		}
	}
}
