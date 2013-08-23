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
			try
			{
				var msg = new NotificationMsg { new_device = arg.ctx.device_id };
				UIChangeSubscriber.Instance.SendMsg(JsonConvert.SerializeObject(msg));
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("OnNewDevice failed", err);
			}
		}

		public void OnFolderAdded(object sender, Manipulation.FolderEventArgs arg)
		{
			try
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
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("OnFolderAdded failed", err);
			}
		}

		public void NotifyRecvStatus_recved(object sender, WebsocketEventArgs arg)
		{
			try
			{
				if (arg.ctx.fileCtx == null || arg.ctx.fileCtx.is_thumbnail)
					return;

				var ctx = arg.ctx;

				notifyRecvStatus_recved(ctx);
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("NotifyRecvStatus failed", err);
			}
		}

		public void ClearRecvStatus(object sender, WebsocketEventArgs arg)
		{
			try
			{
				var ctx = arg.ctx;

				clearRecvStatus(ctx);
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("ClearRecvStatus failed", err);
			}
		}

		private static void clearRecvStatus(ProtocolContext ctx)
		{
			var msg = new NotificationMsg
			{
				recving_devices = new List<ReceivingStatus>()
				{
					new ReceivingStatus {
						DeviceId = ctx.device_id
					}
				}
			};

			UIChangeSubscriber.Instance.SendMsg(JsonConvert.SerializeObject(msg));
		}

		private static void notifyRecvStatus_recved(ProtocolContext ctx)
		{
			var status = new ReceivingStatus
			{
				DeviceId = ctx.device_id,
				IsPreparing = ctx.IsPreparing,
				IsReceiving = ctx.IsRecving,
				Received = (int)ctx.recved_files,
				Total = (int)(ctx.total_count - ctx.backup_count + ctx.recved_files - 1)
				// -1 because backup_count is not yet updated to the latest value in recved / droped state
			};

			if (status.Total == status.Received && status.Total > 0)
				status.IsReceiving = status.IsPreparing = false;


			var msg = new NotificationMsg
			{
				recving_devices = new List<ReceivingStatus> { status }
			};

			UIChangeSubscriber.Instance.SendMsg(JsonConvert.SerializeObject(msg));
		}

		public void OnFileReceived(object sender, WebsocketEventArgs arg)
		{
			try
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
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("OnFileReceived failed", err);
			}
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
