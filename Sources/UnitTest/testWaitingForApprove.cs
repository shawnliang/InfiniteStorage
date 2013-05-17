using InfiniteStorage;
using InfiniteStorage.Model;
using InfiniteStorage.WebsocketProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Frame;

namespace UnitTest
{
	[TestClass]
	public class testWaitingForApprove
	{
		[TestMethod]
		public void testApprove()
		{
			var util = new Mock<IConnectMsgHandlerUtil>();
			util.Setup(x => x.GetServerId()).Returns("server_id1").Verifiable();
			util.Setup(x => x.GetPhotoFolder()).Returns(@"c:\folder1\").Verifiable();
			util.Setup(x => x.GetFreeSpace(It.IsAny<string>())).Returns(123456).Verifiable();
			util.Setup(x => x.GetUniqueDeviceFolder("na")).Returns("ggyyUnique_dev_name").Verifiable();
			util.Setup(x => x.Save(It.Is<Device>(
				(d) =>
					d.device_id == "dd" &&
					d.device_name == "na" &&
					d.folder_name == "ggyyUnique_dev_name"
				))).Callback<Device>((dev) => { }).Verifiable();

			var storage = new Mock<IFileStorage>();
			storage.Setup(x => x.setDeviceName("ggyyUnique_dev_name")).Verifiable();

			var state = new WaitForApproveState();
			var ctx = new ProtocolContext(null, storage.Object, state) { device_id = "dd", device_name = "na" };

			string sentData = null;
			ctx.SendFunc = (t) => { sentData = t; };

			state.Util = util.Object;
			state.handleApprove(ctx);

			var o = JObject.Parse(sentData);
			Assert.AreEqual("accept", o["action"]);
			Assert.AreEqual("server_id1", o["server_id"]);
			Assert.AreEqual(@"c:\folder1\", o["backup_folder"]);
			Assert.AreEqual(123456L, o["backup_folder_free_space"]);

			Assert.IsTrue(ctx.GetState() is TransmitInitState);
			Assert.AreEqual("ggyyUnique_dev_name", ctx.device_folder_name);

			storage.VerifyAll();
			util.VerifyAll();
		}

		[TestMethod]
		public void testDisapprove()
		{
			var state = new WaitForApproveState();
			var ctx = new ProtocolContext(null, null, state) { device_id = "dd", device_name = "na" };

			string sentData = null;
			ctx.SendFunc = (t) => { sentData = t; };

			CloseStatusCode code = CloseStatusCode.NORMAL;
			string reason = null;
			ctx.StopFunc = (code1, reason1) => { code = code1; reason = reason1; };

			state.handleDisapprove(ctx);

			var o = JObject.Parse(sentData);
			Assert.AreEqual("denied", o["action"]);
			Assert.AreEqual("user rejected", o["reason"]);

			Assert.IsTrue(ctx.GetState() is UnconnectedState);
			Assert.AreEqual(CloseStatusCode.POLICY_VIOLATION, code);
			Assert.AreEqual("User rejected", reason);
		}
	}
}
