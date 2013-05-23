using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Model;
using System.Diagnostics;

namespace InfiniteStorage
{
	class PendingFileMonitor
	{
		private static PendingFileMonitor instance = new PendingFileMonitor();

		public Timer timer = new Timer();

		private long showUIAtSeq = 0;
		private long prevSeq = 0;
		private DateTime prevTimeOfNewFileComing = DateTime.Now;
		private FakeSplitEventForm dialog = new FakeSplitEventForm();

		private PendingFileMonitor()
		{
			timer.Interval = 5000;
			timer.Tick += new EventHandler(timer_Tick);
		}

		void timer_Tick(object sender, EventArgs e)
		{
			try
			{
				if (!newFilesComing())
				{
					var duration = DateTime.Now - prevTimeOfNewFileComing;
					if (duration > TimeSpan.FromSeconds(9.0))
					{
						if (showUIAtSeq != prevSeq)
						{
							showUIAtSeq = prevSeq;
							showUI();
						}
					}
				}
				else
				{
					prevTimeOfNewFileComing = DateTime.Now;
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Pending file monitor: ", err);
			}
		}

		private void showUI()
		{
			/*
			if (dialog.IsDisposed)
			{
				dialog = new FakeSplitEventForm();
			}
			dialog.seq = prevSeq;
			dialog.Show();
			dialog.Activate();
			dialog.BringToFront();
			*/

			var devices = getPendingSources();

			foreach (var device in devices)
			{
				ImportUIPresenter.Instance.Show(device);
			}
		}

		private List<string> getPendingSources()
		{
			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.PendingFiles
						where !f.deleted && (f.thumb_ready && f.type == (int)FileAssetType.image || f.type == (int)FileAssetType.video)
						select f.device_id;

				return q.Distinct().ToList();
			}
		}

		private bool newFilesComing()
		{
			using (var db = new MyDbContext())
			{
				var file = (from f in db.Object.PendingFiles
						 where f.seq > prevSeq && (f.thumb_ready && f.type == (int)FileAssetType.image || f.type == (int)FileAssetType.video)
						 orderby f.seq descending
						 select f).Take(1).FirstOrDefault();

				if (file != null)
				{
					prevSeq = file.seq;
					return true;
				}
				else
					return false;
			}
		}

		public static PendingFileMonitor Instance
		{
			get { return instance; }
		}

		public void Start()
		{
			timer.Start();
		}

		public void Stop()
		{
			timer.Stop();
		}
	}
}
