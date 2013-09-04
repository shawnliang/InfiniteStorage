using InfiniteStorage.Model;
using InfiniteStorage.Share;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTest.share
{
	[TestClass]
	public class testMonitor
	{
		Mock<IShareLabelMonitorDB> db;
		Mock<IShareTaskFactory> fac;
		ShareLabelMonitor monitor;

		[TestInitialize]
		public void setup()
		{
			db = new Mock<IShareLabelMonitorDB>();
			fac = new Mock<IShareTaskFactory>();

			monitor = new ShareLabelMonitor(db.Object, fac.Object);
		}

		[TestMethod]
		public void testEnableShare()
		{
			var label = new Label { label_id = Guid.NewGuid(), share_enabled = true, share_proc_seq = 100, seq = 200, deleted = false, auto_type = 0, name = "test" };
			db.Setup(x => x.QueryLabelsNeedingProcess()).Returns(new List<Label> { 
				label
			});


			Mock<IShareTask> task = new Mock<IShareTask>();
			task.Setup(x => x.Process(label)).Verifiable();

			fac.Setup(x => x.CreateEnableTask()).Returns(task.Object);

			monitor.Run();

			task.VerifyAll();
		}

		[TestMethod]
		public void testDisableShare()
		{
			var label = new Label { label_id = Guid.NewGuid(), share_enabled = false, share_proc_seq = 100, seq = 200, deleted = false, auto_type = 0, name = "test" };
			db.Setup(x => x.QueryLabelsNeedingProcess()).Returns(new List<Label> { 
				label
			});

			Mock<IShareTask> task = new Mock<IShareTask>();
			task.Setup(x => x.Process(label)).Verifiable();

			fac.Setup(x => x.CreateDisableTask()).Returns(task.Object);

			monitor.Run();

			task.VerifyAll();
		}

		[TestMethod]
		public void one_task_failure_wont_skip_another()
		{
			db.Setup(x => x.QueryLabelsNeedingProcess()).Returns(new List<Label>{
						 new Label{ label_id = Guid.NewGuid(), share_enabled = true},
						 new Label{ label_id = Guid.NewGuid(), share_enabled = false}
			});

			Mock<IShareTask> task1 = new Mock<IShareTask>();
			task1.Setup(x => x.Process(It.IsAny<Label>())).Throws(new Exception()).Verifiable();

			Mock<IShareTask> task2 = new Mock<IShareTask>();
			task2.Setup(x => x.Process(It.IsAny<Label>())).Verifiable();

			fac.Setup(x => x.CreateEnableTask()).Returns(task1.Object);
			fac.Setup(x => x.CreateDisableTask()).Returns(task2.Object);

			monitor.Run();

			task1.VerifyAll();
			task2.VerifyAll();
		}
	}
}
