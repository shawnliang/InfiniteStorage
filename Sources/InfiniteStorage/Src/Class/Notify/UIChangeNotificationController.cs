using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Data.Notify;
using Newtonsoft.Json;
using InfiniteStorage.Model;

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

		public void OnFileReceived(object sender, WebsocketEventArgs arg)
		{
			if (arg.ctx.fileCtx.is_thumbnail)
				return;

			var file_id = arg.ctx.fileCtx.file_id;

			Folder folder;
			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.Files
						join dir in db.Object.Folders on f.parent_folder equals dir.path
						where f.file_id == file_id
						select dir;
				folder = q.FirstOrDefault();
			}

			if (folder == null)
				return;

			NotifyFolderUpdate(folder);
		}

		public static void NotifyFolderUpdate(Folder folder)
		{
			var msg = new NotificationMsg
			{
				update_folder = new folder_info
				{
					name = folder.name,
					parent_folder = folder.parent_folder,
					path = folder.path
				}
			};

			UIChangeSubscriber.Instance.SendMsg(JsonConvert.SerializeObject(msg));
		}
	}
}
