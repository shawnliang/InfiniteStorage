using InfiniteStorage.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfiniteStorage.Share
{
	class ShareLabelMonitorDB : IShareLabelMonitorDB
	{
		public ICollection<Model.Label> QueryLabelsNeedingProcess()
		{
			using (var db = new MyDbContext())
			{
				var starred = Guid.Empty;

				var q = from lb in db.Object.Labels
						where lb.seq > lb.share_proc_seq &&
								!lb.deleted &&
								lb.auto_type == (int)AutoLabelType.NotAuto &&
								lb.label_id != starred
						select lb;

				return q.ToList();
			}
		}
	}
}
