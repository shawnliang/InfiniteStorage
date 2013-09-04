using InfiniteStorage.Model;
using System.Collections.Generic;

namespace InfiniteStorage.Share
{
	public interface IShareLabelMonitorDB
	{
		ICollection<Label> QueryLabelsNeedingProcess();
	}
}
