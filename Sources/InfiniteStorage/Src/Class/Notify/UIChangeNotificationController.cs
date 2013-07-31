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
			var msg = new NotificationMsg { new_device = arg.ctx.device_id };
			UIChangeSubscriber.Instance.SendMsg(JsonConvert.SerializeObject(msg));
		}

		public void OnFolderAdded(object sender, Manipulation.FolderEventArgs arg)
		{
			var msg = new NotificationMsg
			{
				new_folder = new folder_info
				{
					name = arg.name,
					parent_folder = arg.parent_folder,
					path = arg.path
				}
			};

			UIChangeSubscriber.Instance.SendMsg(JsonConvert.SerializeObject(msg));
		}
	}
}
