using InfiniteStorage.Notify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
	}
}
