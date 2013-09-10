using InfiniteStorage.Data.Notify;
using InfiniteStorage.Model;
using InfiniteStorage.WebsocketProtocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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

		public void NotifyRecvStatus_recving(object sender, WebsocketEventArgs arg)
		{
			try
			{
				var ctx = arg.ctx;

				if (ctx.fileCtx == null)
					return;

				if (ctx.fileCtx.is_thumbnail)
				{
					notifyRecvStatus(new ReceivingStatus { DeviceId = ctx.device_id, IsPreparing = true });
				}
				else
				{
					notifyRecvStatus(new ReceivingStatus
					{
						DeviceId = ctx.device_id,
						IsReceiving = true,
						Total = (int)(ctx.total_count - ctx.backup_count + ctx.recved_files),
						Received = (int)ctx.recved_files
					});
				}


			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("NotifyRecvStatus failed", err);
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
			notifyRecvStatus(new ReceivingStatus() { DeviceId = ctx.device_id });
		}

		private static void notifyRecvStatus(ReceivingStatus status)
		{
			var msg = new NotificationMsg
			{
				recving_devices = new List<ReceivingStatus> { status }
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

			if (status.Total == status.Received && status.IsReceiving)
				status.IsReceiving = status.IsPreparing = false;


			notifyRecvStatus(status);
		}

		public void OnFilesFlushedToDB(object sender, FilesFlushedEventArgs arg)
		{
			try
			{
				var folders = arg.files.Where(x => x.has_origin).Select(x => x.parent_folder).Distinct().Select(x => new Folder
				{
					path = x,
					name = Path.GetFileName(x),
					parent_folder = Path.GetDirectoryName(x)
				});

				foreach (var folder in folders)
					NotifyFolderUpdate(folder);
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("OnFilesFlushedToDB failed", err);
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
