using InfiniteStorage.Data.Notify;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using Waveface.Model;
using WebSocketSharp;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Diagnostics;

namespace Waveface.ClientFramework
{
	public class BunnyServiceSupplier : ServiceSupplier
	{
		#region Static Var
		private static BunnyServiceSupplier _default;
		private SynchronizationContext syncContext;
		#endregion


		#region Public Static Property
		public static BunnyServiceSupplier Default
		{
			get
			{
				return _default ?? (_default = new BunnyServiceSupplier());
			}
		}
		#endregion


		#region Protected Property
		protected override ServiceSupplierInfo m_Info
		{
			get { throw new NotImplementedException(); }
		}
		#endregion


		#region Public Property
		public override string ID
		{
			get { throw new NotImplementedException(); }
		}
		#endregion


		#region Constructor
		private BunnyServiceSupplier()
		{
			InitServices();
			syncContext = SynchronizationContext.Current;
		}
		#endregion


		#region Private Var
		WebSocket ws;
		private bool ws_inited;
		#endregion

		#region Private Method
		private void InitServices()
		{
			this.Services.AddRange(GetServices());

			if (!ws_inited)
			{
				subscribeActiveDeviceAsync();
				ws_inited = true;
			}
		}

		private List<Service> GetServices()
		{
			var services = new List<Service>();

			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT * FROM Devices order by folder_name";

					using (var dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							var folderName = dr["folder_name"].ToString();
							var deviceId = dr["device_id"].ToString();
							var syncDisabled = (bool)dr["sync_disabled"];
							services.Add(new BunnyService(this, folderName, deviceId, !syncDisabled));
						}
					}
				}

				return services;
			}
		}

		private void subscribeActiveDeviceAsync()
		{
			System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
			{

				var port = Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "notify_port", "13995");

				var wsUrl = string.Format("ws://127.0.0.1:{0}/", port);

				ws = new WebSocket(wsUrl, new string[] { });
				ws.OnOpen += new EventHandler(ws_OnOpen);
				ws.OnClose += new EventHandler<CloseEventArgs>(ws_OnClose);
				ws.OnMessage += new EventHandler<MessageEventArgs>(ws_OnMessage);


				Observable.FromEvent<EventHandler<MessageEventArgs>, MessageEventArgs>(
				handler => (s, ex) => handler(ex),
				h => ws.OnMessage += h,
				h => ws.OnMessage -= h
				)
				.Where(ex =>
				{
					var notify = JsonConvert.DeserializeObject<NotificationMsg>(ex.Data);
					return notify.update_folder != null;
				})
				.Window(TimeSpan.FromMilliseconds(5 * 1000))
				.Subscribe((events) =>
				{
					events
						.Distinct(ex =>
						{
							var notify = JsonConvert.DeserializeObject<NotificationMsg>(ex.Data);
							return notify.update_folder.path;
						})
						.Subscribe(ex =>
						{
							var notify = JsonConvert.DeserializeObject<NotificationMsg>(ex.Data);

							refreshFolder(notify.update_folder);
						});
				});


				System.Threading.Thread.Sleep(1000);
				ws.Connect();
			});
			t.Start();
		}

		void ws_OnOpen(object sender, EventArgs e)
		{
			var msg = new
			{
				connect = new
				{
					device_id = Guid.NewGuid().ToString(),
					device_name = "MainUI"
				},
				subscribe = new
				{
					devices = true
				}
			};

			ws.Send(JsonConvert.SerializeObject(msg));
		}

		void ws_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Type != WebSocketSharp.Frame.Opcode.TEXT)
				return;

			try
			{
				var notify = JsonConvert.DeserializeObject<NotificationMsg>(e.Data);

				if (notify.recving_devices != null)
				{
					updateServiceReceivingStatus(notify);
				}

				if (!string.IsNullOrEmpty(notify.new_device))
				{
					addNewDevice(notify.new_device);
				}

				if (notify.new_folder != null)
				{
					addNewFolder(notify.new_folder);
				}

				if (notify.update_folder != null)
				{
					// moved to subscribeActiveDeviceAsync()
				}
			}
			catch (Exception err)
			{

			}
		}

		private void refreshFolder(folder_info folder)
		{
			// wrap the following find-and-modify logic in syncContext to avoid locking
			syncContext.Post((dummy) =>
			{
				// This is not always true in multi-level folder hiararchy.
				// But currently we have only two level hiararchy (device and folder),
				// So this is valid currently.
				var device_name = folder.parent_folder;
				var device = Services.Where(x => x.Name == device_name).FirstOrDefault();

				if (device == null)
					return;

				if (!device.Contents.Where(x => x.Name == folder.name).Any())
				{
					var dev = device as BunnyService;
					dev.AddContent(new BunnyContentGroup(dev.Name, folder.name, dev.ID) { Service = dev });
				}
				else
				{
					var group = device.Contents.Where(x => x.Name == folder.name).First();
					group.Refresh();
				}

			}, null);
		}

		private void addNewFolder(folder_info folder)
		{
			// wrap the following find-and-modify logic in syncContext to avoid locking
			syncContext.Post((dummy) =>
			{
				// This is not always true in multi-level folder hiararchy.
				// But currently we have only two level hiararchy (device and folder),
				// So this is valid currently.
				var device_name = folder.parent_folder;
				var device = Services.Where(x => x.Name == device_name).FirstOrDefault();

				if (device == null)
					return;

				if (!device.Contents.Where(x => x.Name == folder.name).Any())
				{
					var dev = device as BunnyService;
					dev.AddContent(new BunnyContentGroup(dev.Name, folder.name, dev.ID) { Service = dev });
				}

			}, null);
		}

		private void addNewDevice(string device_id)
		{
			var q = from s in Services
					where s.ID == device_id
					select 1;

			if (q.Any())
				return;


			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select folder_name, sync_disabled from [devices] where device_id = @dev";
					cmd.Parameters.Add(new SQLiteParameter("@dev", device_id));

					using (var reader = cmd.ExecuteReader())
					{
						var dev_folder = reader["folder_name"].ToString();
						var sync_disabled = (bool)reader["sync_disabled"];

						if (dev_folder != null)
						{
							var service = new BunnyService(this, dev_folder.ToString(), device_id, !sync_disabled);

							syncContext.Post((dummy) =>
							{
								Services.Add(service);
							}, null);
						}
					}
				}
			}
		}

		private void updateServiceReceivingStatus(NotificationMsg notify)
		{
			var recving_devices = notify.recving_devices;

			var services = Services.ToList();

			var actives = from s in services
						  where recving_devices.Where(x => x.DeviceId.Equals(s.ID, StringComparison.InvariantCultureIgnoreCase)).Any()
						  select s;

			foreach (BunnyService s in actives)
			{
				var status = recving_devices.Where(x => x.DeviceId.Equals(s.ID, StringComparison.InvariantCultureIgnoreCase)).First();
				s.RecvStatus.IsReceiving = s.IsRecving = status.IsReceiving;
				s.RecvStatus.IsPreparing = status.IsPreparing;
				s.RecvStatus.Total = status.Total;
				s.RecvStatus.Received = status.Received;
			}
		}

		void ws_OnClose(object sender, CloseEventArgs e)
		{
			subscribeActiveDeviceAsync();
		}
		#endregion
	}
}
