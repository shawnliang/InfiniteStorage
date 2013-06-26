using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Share
{
	class ShareDisableTask : IShareTask
	{
		public void Process(Model.Label label)
		{
			var db = new ShareEnableTaskDB();
			db.UpdateShareComplete(label);
		}
	}
}
