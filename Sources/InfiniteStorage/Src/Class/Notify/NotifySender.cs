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
			var result = util.QueryChangedFiles(from_seq);

			var all = result.ToList();

			if (all.Count > 0)
			{
				var msg = JsonConvert.SerializeObject(new
				   {
					   file_changes = all
				   });

				ctx.Send(msg);

				from_seq = all.Last().seq + 1;
			}
		}
	}
}
