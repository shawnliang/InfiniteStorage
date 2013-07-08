using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Share
{
	public interface IShareTaskFactory
	{
		IShareTask CreateEnableTask();
		IShareTask CreateDisableTask();
	}

	public interface IShareTask
	{
		void Process(InfiniteStorage.Model.Label label);
	}
}
