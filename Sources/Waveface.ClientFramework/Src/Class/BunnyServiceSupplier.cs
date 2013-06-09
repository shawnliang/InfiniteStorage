using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Waveface.Model;
using WebSocketSharp;
using Newtonsoft.Json;

namespace Waveface.ClientFramework
{
	public class BunnyServiceSupplier : ServiceSupplier
	{
		#region Static Var
		private static BunnyServiceSupplier _default;
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
			SetServices();
		}
		#endregion


		#region Private Var
		WebSocket ws;
		private bool ws_inited;
		#endregion

		#region Private Method
		private void SetServices()
		{
			var services = new List<Service>();
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var dbFilePath = Path.Combine(appDir, "database.s3db");

			var conn = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

			conn.Open();

			var cmd = new SQLiteCommand("SELECT * FROM Devices", conn);

			var dr = cmd.ExecuteReader();

			while (dr.Read())
			{
				var folderName = dr["folder_name"].ToString();
				var deviceId = dr["device_id"].ToString();
				services.Add(new BunnyService(this, folderName, deviceId));
			}

			conn.Close();

			this.Services = services;

			if (!ws_inited)
			{
				subscribeActiveDeviceAsync();
				ws_inited = true;
			}
		}

		private void subscribeActiveDeviceAsync()
		{
			System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() => {
				ws = new WebSocket("ws://127.0.0.1:13995/", new string[] { });
				ws.OnOpen += new EventHandler(ws_OnOpen);
				ws.OnClose += new EventHandler<CloseEventArgs>(ws_OnClose);
				ws.OnMessage += new EventHandler<MessageEventArgs>(ws_OnMessage);


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
				subscribe = new {
					devices = true
				}
			};

			ws.Send(JsonConvert.SerializeObject(msg));
		}

		void ws_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Type != WebSocketSharp.Frame.Opcode.TEXT)
				return;

			try{
				var notify = JsonConvert.DeserializeObject<NotifyMsg>(e.Data);
				var active_devices = notify.active_devices;

				var services = this.Services;

				var actives = from s in services
							  where active_devices.Contains(s.ID)
							  select s;

				foreach (BunnyService s in actives)
					s.IsRecving = true;

				var inactives = services.Except(actives);

				foreach (BunnyService s in inactives)
					s.IsRecving = false;
			}
			catch(Exception err)
			{
			
			}
		}

		void ws_OnClose(object sender, CloseEventArgs e)
		{
			subscribeActiveDeviceAsync();
		}
		#endregion
	}


	internal class NotifyMsg
	{
		public List<string> active_devices { get; set; }
	}
}
