using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Common;

namespace InfiniteStorage.Share
{
	public class ShareLabelMonitor
	{
		private IShareLabelMonitorDB db;
		private IShareTaskFactory factory;

		public ShareLabelMonitor()
		{
			this.db = new ShareLabelMonitorDB();
			this.factory = new ShareTaskFactory();
		}

		public ShareLabelMonitor(IShareLabelMonitorDB db, IShareTaskFactory factory)
		{
			this.db = db;
			this.factory = factory;
		}

		public void Run()
		{
			var labels = db.QueryLabelsNeedingProcess();
			
			foreach (var label in labels)
			{
				IShareTask task;
				if (label.share_enabled.HasValue && label.share_enabled.Value)
					task = factory.CreateEnableTask();
				else
					task = factory.CreateDisableTask();

				try
				{
					task.Process(label);
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to enable/disable share on label: " + label.label_id.ToString() + " " + label.name, err);
				}
			}
		}
	}
}
