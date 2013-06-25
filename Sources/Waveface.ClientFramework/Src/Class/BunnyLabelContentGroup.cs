using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Model;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;

namespace Waveface.ClientFramework
{
	public class BunnyLabelContentGroup : ContentGroup
	{
		public bool ShareEnabled { get; private set; }

		public string ShareURL
		{
			get {
				if (!string.IsNullOrWhiteSpace(m_shareCode))
					return "https://devweb.waveface.com/favorite/" + m_shareCode; // TODO: remove hard code;
				else
					return string.Empty;
			}
		}

		public ReadOnlyObservableCollection<BunnyRecipient> Recipients
		{
			get { return m_readonlyRecipients; }
		}


		private string m_shareCode { get; set; }
		private ObservableCollection<BunnyRecipient> m_recipients = new ObservableCollection<BunnyRecipient>();
		private ReadOnlyObservableCollection<BunnyRecipient> m_readonlyRecipients;


		public BunnyLabelContentGroup(string label_id, string name, bool shareEnabled, string shared_code)
			:base(label_id, name, new Uri("label://" + label_id))
		{
			SetContents(populateContents);
			this.ShareEnabled = shareEnabled;
			this.m_shareCode = shared_code;

			this.m_readonlyRecipients = new ReadOnlyObservableCollection<BunnyRecipient>(m_recipients);

			if (this.ShareEnabled)
				refreshRecipients();
		}

		private void refreshRecipients()
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = "select * from [LabelShareTo] where label_id = @label";
				cmd.Parameters.Add(new SQLiteParameter("@label", new Guid(this.ID)));

				using (var reader = cmd.ExecuteReader())
				{
					var email = reader["email"].ToString();
					var name = reader["name"].ToString();

					var recipient = new BunnyRecipient(email, name);

					if (!m_recipients.Contains(recipient))
						m_recipients.Add(recipient);
				}
			}
		}

		private void populateContents(ObservableCollection<IContentEntity> contents)
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				var cmd2 = conn.CreateCommand();
				cmd2.CommandText = "SELECT * FROM Files t1, LabelFiles t2, Labels t3 where t3.label_id = @labelID and t3.label_id = t2.label_id and t1.file_id = t2.file_id order by t1.event_time asc";
				cmd2.Parameters.Add(new SQLiteParameter("@labelID", new Guid(this.ID)));

				var dr2 = cmd2.ExecuteReader();

				while (dr2.Read())
				{
					var deviceID = dr2["device_id"].ToString();
					var savedPath = dr2["saved_path"].ToString();
					var file = Path.Combine(BunnyDB.ResourceFolder, savedPath);

					var type = ((long)dr2["type"] == 0) ? ContentType.Photo : ContentType.Video;
					contents.Add(new BunnyContent(new Uri(file), dr2["file_id"].ToString(), type));
				}
			}
		}

		public override void Refresh()
		{
			base.Refresh();

			refreshShareProperties();

			refreshRecipients();
		}

		private void refreshShareProperties()
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = "select * from [labels] where label_id = @label";
				cmd.Parameters.Add(new SQLiteParameter("@label", new Guid(this.ID)));
				using (var reader = cmd.ExecuteReader())
				{
					var share_enabled = (bool)reader["share_enabled"];
					var share_code = reader["share_code"].ToString();

					if (share_enabled != this.ShareEnabled)
					{
						this.ShareEnabled = share_enabled;
						OnPropertyChanged("ShareEnabled");
					}

					if (share_code != this.m_shareCode)
					{
						this.m_shareCode = share_code;
						OnPropertyChanged("ShareURL");
					}
				}
			}
		}
	}
}
