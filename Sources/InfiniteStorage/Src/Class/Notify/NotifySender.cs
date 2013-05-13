using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace InfiniteStorage.Notify
{
	public class NotifySender
	{
		private INotifySenderUtil util;

		public ISubscriptionContext ctx { get; private set; }
		public long from_seq { get; private set; }

		public NotifySender(ISubscriptionContext ctx, INotifySenderUtil util)
		{
			this.ctx = ctx;
			this.util = util;
			this.from_seq = ctx.files_from_seq;
		}

		public void Notify()
		{
			var all = util.QueryChangedFiles(from_seq);

			if (all.Count > 0)
			{
				int nSent = 0;

				do
				{
					var batch = all.Skip(nSent).Take(500).ToList();

					var msg = JsonConvert.SerializeObject(new
					{
						file_changes = batch
					});

					ctx.Send(msg);
					nSent += batch.Count;

				} while (nSent < all.Count);

				from_seq = all.Last().seq + 1;
			}
		}
	}
}
