using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Share
{
	class ShareTaskFactory : IShareTaskFactory
	{
		public IShareTask CreateEnableTask()
		{
			return new ShareEnableTask();
		}

		public IShareTask CreateDisableTask()
		{
			return new ShareDisableTask();
		}
	}
}
