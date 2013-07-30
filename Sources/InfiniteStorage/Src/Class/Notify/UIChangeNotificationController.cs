using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Data.Notify;
using Newtonsoft.Json;

namespace InfiniteStorage.Notify
{
	class UIChangeNotificationController
	{
		public void OnSubscribingUIChanges(object sender, NotifyChannelEventArgs arg)
		{
			UIChangeSubscriber.Instance.Add(arg.Ctx);
		}

		public void OnEndingSubscription(object sender, NotifyChannelEventArgs arg)
		{
			UIChangeSubscriber.Instance.Remove(arg.Ctx);
		}

		public void OnNewDevice(object sender, WebsocketEventArgs arg)
		{
			var msg = new NotificationMsg { NewDevice = arg.ctx.device_id };
			UIChangeSubscriber.Instance.SendMsg(JsonConvert.SerializeObject(msg));
		}
	}
}
