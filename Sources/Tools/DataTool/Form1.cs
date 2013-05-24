using InfiniteStorage.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using WebSocketSharp;

namespace DataTool
{
	public partial class Form1 : Form
	{
		WebSocket client;

		public Form1()
		{
			InitializeComponent();
		}

		private void generateButton_Click(object sender, EventArgs e)
		{
			using (var conn = new SQLiteConnection())
			{
				conn.ConnectionString = MyDbContext.ConnectionString;
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				{
					var cmd = new SQLiteCommand("delete from [files]; delete from [devices]; delete from [labels]; delete from [labelFiles];");
					cmd.Connection = conn;
					cmd.ExecuteNonQuery();

					var dev_id = Guid.NewGuid().ToString();

					cmd.CommandText = "insert into [devices] (device_id, device_name, folder_name) values (@id, 'dev_name', 'dev_folder')";
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Clear();
					cmd.Parameters.Add(new SQLiteParameter("@id", dev_id));
					cmd.ExecuteNonQuery();


					for (int i = 0; i < int.Parse(filesCountTxtBox.Text); i++)
					{
						var cmd1 = new SQLiteCommand(conn);
						cmd1.CommandText =
							"insert into [files] " +
							"(device_id, event_time, file_id, file_name, file_path, file_size, saved_path, seq, thumb_ready, type) " +
							"values (@devid, @evtTime, @fid, @fname, @fpath, @fsize, @path, @seq, 0, 0)";
						cmd1.CommandType = CommandType.Text;
						cmd1.Parameters.Add(new SQLiteParameter("@devid", dev_id));
						cmd1.Parameters.Add(new SQLiteParameter("@evtTime", DateTime.Now));
						cmd1.Parameters.Add(new SQLiteParameter("@fid", Guid.NewGuid()));
						cmd1.Parameters.Add(new SQLiteParameter("@fname", "file" + i + ".jpg"));
						cmd1.Parameters.Add(new SQLiteParameter("@fpath", "path" + i + ".jpg"));
						cmd1.Parameters.Add(new SQLiteParameter("@fsize", 12345678));
						cmd1.Parameters.Add(new SQLiteParameter("@path", @"2012\2012-10\file" + i + ".jpg"));
						cmd1.Parameters.Add(new SQLiteParameter("@seq", i + 1));

						cmd1.ExecuteNonQuery();
					}

					var label_id = Guid.NewGuid();
					cmd.CommandText = "insert into [labels] (label_id, name, seq) values (@labelId, 'no name', 120)";
					cmd.Parameters.Clear();
					cmd.Parameters.Add(new SQLiteParameter("@labelId", label_id));
					cmd.ExecuteNonQuery();


					cmd.CommandText = "insert into [labelFiles] (label_id, file_id) " +
									"select @labelId, file_id from [Files] where seq < 10";
					cmd.Parameters.Clear();
					cmd.Parameters.Add(new SQLiteParameter("@labelId", label_id));
					cmd.ExecuteNonQuery();


					transaction.Commit();
				}
			}
		}

		private void connectButton_Click(object sender, EventArgs e)
		{
			client = new WebSocket(textBox1.Text);
			client.OnMessage += client_OnMessage;
			client.OnError += new EventHandler<ErrorEventArgs>(client_OnError);
			client.Connect();

			object data;

			if (subcribeFiles.Checked)
				data = new
				{
					connect = new
					{
						device_id = Guid.NewGuid().ToString(),
						device_name = "shawn's test tool"
					},

					subscribe = new
					{
						files_from_seq = Int64.Parse(fileSeq.Text),
						labels = subscribeLabels.Checked,
						labels_from_seq = int.Parse(label_seq.Text),

					}
				};
			else
				data = new
				{
					connect = new
					{
						device_id = Guid.NewGuid().ToString(),
						device_name = "shawn's test tool"
					},

					subscribe = new
					{
						labels = subscribeLabels.Checked,
						labels_from_seq = int.Parse(label_seq.Text),
					}
				};


			client.Send(JsonConvert.SerializeObject(data));
		}

		void client_OnError(object sender, ErrorEventArgs e)
		{
			var error = "";
		}

		void client_OnMessage(object sender, MessageEventArgs e)
		{
			if (textBoxNotify.InvokeRequired)
			{
				textBoxNotify.Invoke(new MethodInvoker(() =>
				{
					client_OnMessage(sender, e);
				}));

				return;
			}

			var o = JObject.Parse(e.Data);
			var files = o["file_changes"];

			if (files != null)
			{
				foreach (var file in files)
				{
					textBoxNotify.AppendText(file["file_name"].Value<string>() + " => " + file["id"].Value<string>() + "\r\n");
				}
			}

			var label = o["label_change"];
			if (label != null)
			{
				textBoxNotify.AppendText("\r\n" + e.Data + "\r\n");
			}


			//textBoxNotify.AppendText(e.Data);
		}

		private void disconnectButton_Click(object sender, EventArgs e)
		{
			if (client != null)
			{
				client.OnMessage -= client_OnMessage;
				client.Close();
				client = null;
			}
		}
	}
}
