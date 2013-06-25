using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using InfiniteStorage.Model;

namespace InfiniteStorage.REST
{
	class LabelInviteApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("label_id", "recipients");

			var label_id = new Guid(Parameters["label_id"]);
			var recipients = getRecipients();
			var sender = Parameters["sender"];
			var msg = Parameters["msg"];

			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				{
					var cmdQuery = conn.CreateCommand();
					cmdQuery.CommandText = "select 1 from [LabelShareTo] where label_id = @label and email = @email";
					cmdQuery.Prepare();

					var cmdInsert = conn.CreateCommand();
					cmdInsert.CommandText = "insert into [LabelShareTo] (label_id, email, name) values (@label, @email, @name)";
					cmdInsert.Prepare();

					bool labelChanged = false;

					foreach (var recipient in recipients)
					{
						cmdQuery.Parameters.Clear();
						cmdQuery.Parameters.Add(new SQLiteParameter("@label", label_id));
						cmdQuery.Parameters.Add(new SQLiteParameter("@email", recipient.email));

						var recipientExists = cmdQuery.ExecuteScalar() != null;

						if (!recipientExists)
						{
							cmdInsert.Parameters.Clear();
							cmdInsert.Parameters.Add(new SQLiteParameter("@label", label_id));
							cmdInsert.Parameters.Add(new SQLiteParameter("@email", recipient.email));
							cmdInsert.Parameters.Add(new SQLiteParameter("@name", recipient.name));
							cmdInsert.ExecuteNonQuery();
							labelChanged = true;
						}
					}


					if (labelChanged)
					{
						var cmd = conn.CreateCommand();
						cmd.CommandText = "update labels set seq = @seq where label_id = @label";
						cmd.Parameters.Add(new SQLiteParameter("@seq", (object)SeqNum.GetNextSeq()));
						cmd.Parameters.Add(new SQLiteParameter("@label", label_id));
						var affectedRows = cmd.ExecuteNonQuery();

						if (affectedRows == 0)
							throw new Exception("label_id does not exist: " + label_id.ToString());
					}

					transaction.Commit();
				}
			}


			respondSuccess();
		}

		private List<Recipient> getRecipients()
		{
			List<Recipient> recipients;
			try
			{
				recipients = JsonConvert.DeserializeObject<List<Recipient>>(Parameters["recipients"]);
			}
			catch (Exception err)
			{
				throw new FormatException("recipient format is invalid: " + Parameters["recipients"], err);
			}
			return recipients;
		}
	}

	class Recipient
	{
		public string email { get; set; }
		public string name { get; set; }
	}
}
