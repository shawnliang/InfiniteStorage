using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using InfiniteStorage.Notify;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnitTest.Notify
{
	[TestClass]
	public class testNotifyDeviceInfo
	{
		private Mock<INotifySenderUtil> util;
		private Mock<ISubscriptionContext> subscribeCtx;
		private NotifySender sender;

		private string sentData;

		[TestInitialize]
		public void setup()
		{
			util = new Mock<INotifySenderUtil>();
			subscribeCtx = new Mock<ISubscriptionContext>();
			subscribeCtx.Setup(x => x.Send(It.IsAny<string>())).Callback<string>((y) => { sentData = y; });
			sender = new NotifySender(subscribeCtx.Object, util.Object);
			sentData = null;

		}

		[TestMethod]
		public void no_subsribe_device()
		{

			subscribeCtx.Setup(x => x.subscribe_devices).Returns(false);
			sender.Notify();

			Assert.IsNull(sentData);
		}

		[TestMethod]
		public void no_dev_is_recving()
		{
			subscribeCtx.Setup(x => x.subscribe_devices).Returns(true);
			sender.Notify();

			Assert.IsNotNull(sentData);
			var o = JsonConvert.DeserializeObject<ActiveDeviceNotifyMsg>(sentData);
			Assert.AreEqual(0, o.active_devices.Count);
		}


		[TestMethod]
		public void a_is_recving()
		{
			var conn = new Mock<IConnectionStatus>();
			conn.Setup(x => x.device_id).Returns("a");
			conn.Setup(x => x.device_name).Returns("name_a");
			conn.Setup(x => x.IsRecving).Returns(true);

			subscribeCtx.Setup(x => x.subscribe_devices).Returns(true);
			util.Setup(x => x.GetAllBackupConnections()).Returns(new List<IConnectionStatus> {
				conn.Object
			});
			sender.Notify();

			Assert.IsNotNull(sentData);
			var o = JsonConvert.DeserializeObject<ActiveDeviceNotifyMsg>(sentData);
			Assert.AreEqual(1, o.active_devices.Count);
			Assert.AreEqual("a", o.active_devices.First());
		}
	}
}
