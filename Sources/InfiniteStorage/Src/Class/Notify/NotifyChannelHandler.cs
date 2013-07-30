using Newtonsoft.Json.Linq;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using InfiniteStorage.Data.Notify;

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
		private const string PROPERTY_DEVICES = "devices";
		private const string PROPERTY_LABELS_FROM_SEQ = "labels_from_seq";

		public event EventHandler<NotifyChannelEventArgs> Subscribing;

		public SubscriptionContext Ctx { get; set; }


		public void HandleMessage(WebSocketService svc, MessageEventArgs args)
		{
			var msg = JsonConvert.DeserializeObject<SubscribeMsg>(args.Data);

			if (Ctx != null)
				throw new WebsocketProtocol.ProtocolErrorException("already connected");

			if (msg.connect == null)
				throw new WebsocketProtocol.ProtocolErrorException("connect is expected");

			if (msg.subscribe == null)
				throw new WebsocketProtocol.ProtocolErrorException("subscribe is expected");

			Ctx = new SubscriptionContext(
				msg.connect.device_id,
				msg.connect.device_name,
				svc);

			var subsribe = msg.subscribe;
			if (subsribe.files_from_seq.HasValue)
			{
				Ctx.subscribe_files = true;
				Ctx.files_from_seq = subsribe.files_from_seq.Value;
			}

			if (subsribe.labels)
			{
				Ctx.subscribe_labels = subsribe.labels;
				Ctx.labels_from_seq = subsribe.labels_from_seq.HasValue ? subsribe.labels_from_seq.Value : 0;
			}

			if (subsribe.devices)
			{
				Ctx.subscribe_devices = subsribe.devices;
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
