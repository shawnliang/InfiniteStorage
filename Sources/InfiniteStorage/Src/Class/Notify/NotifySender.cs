using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfiniteStorage.Notify
{
	public class NotifySender
	{
		private INotifySenderUtil util;

		public ISubscriptionContext ctx { get; private set; }
		public long file_seq { get; private set; }
		public Dictionary<Guid, long> label_seq { get; private set; }

		public NotifySender(ISubscriptionContext ctx, INotifySenderUtil util)
		{
			this.ctx = ctx;
			this.util = util;
			this.file_seq = ctx.files_from_seq;
			this.label_seq = new Dictionary<Guid, long>();
		}

		public void Notify()
		{
			if (ctx.subscribe_files)
			{
				sendChangedFiles();
			}

			if (ctx.subscribe_labels)
			{
				sendChangedLabels();
			}
		}

		private void sendChangedLabels()
		{
			var labels = util.QueryAllLabels();

			foreach (var label in labels)
			{
				var oldSeq = getOldLabelSeq(label);

				if (oldSeq < label.seq)
				{
					try
					{
						ctx.Send(JsonConvert.SerializeObject(new
							{
								label_change = new
								{
									label_id = label.label_id,
									label_name = label.name,
									deleted = label.deleted,
									seq = label.seq,
									cover_url = "/label_cover/" + label.label_id.ToString() + "?seq=" + label.seq,
									auto_type = label.auto_type,
									on_air = label.on_air
								}
							}));

						setOldLabelSeq(label);
					}
					catch (Exception err)
					{
						log4net.LogManager.GetLogger(GetType()).Warn("Unable to send labels to " + ctx.device_name, err);
					}
				}
			}
		}

		private long getOldLabelSeq(Model.Label label)
		{
			var oldSeq = ctx.labels_from_seq;
			if (label_seq.ContainsKey(label.label_id))
				oldSeq = label_seq[label.label_id];
			return oldSeq;
		}

		private void setOldLabelSeq(Model.Label label)
		{
			if (!label_seq.ContainsKey(label.label_id))
				label_seq.Add(label.label_id, label.seq);
			else
				label_seq[label.label_id] = label.seq;
		}

		private void sendChangedFiles()
		{
			var all = util.QueryChangedFiles(file_seq);

			if (all.Count > 0)
			{
				int nSent = 0;

				int page_no = 1;
				int page_count = (all.Count % 500 != 0) ? all.Count / 500 + 1 : all.Count / 500;

				do
				{
					var batch = all.Skip(nSent).Take(500).ToList();

					var msg = JsonConvert.SerializeObject(new
					{
						total_count = all.Count,
						page_count = page_count,
						page_no = page_no++,
						page_size = batch.Count,

						file_changes = batch,
					});

					ctx.Send(msg);
					nSent += batch.Count;

				} while (nSent < all.Count);

				file_seq = all.Last().seq + 1;
			}
		}
	}
}