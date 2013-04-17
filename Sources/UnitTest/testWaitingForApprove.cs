using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage;
using InfiniteStorage.WebsocketProtocol;
using Moq;
using InfiniteStorage.Model;
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
			util.Setup(x=>x.GetFreeSpace(It.IsAny<string>())).Returns(123456).Verifiable();
			util.Setup(x => x.Save(It.Is<Device>(
				(d) =>
					d.audio_count == 0 &&
					d.device_id == "dd" &&
					d.device_name == "na" &&
					d.photo_count == 0 &&
					d.video_count == 0

				))).Callback<Device>((dev) => { }).Verifiable();

			var state = new WaitForApproveState();
			var ctx = new ProtocolContext(null, null, state);
			ctx.device_id = "dd";
			ctx.device_name = "na";
			
			string sentData = null;
			ctx.SendText = (t) => { sentData = t; };

			state.Util = util.Object;
			state.handleApprove(ctx);

			var o = JObject.Parse(sentData);
			Assert.AreEqual("accept", o["action"]);
			Assert.AreEqual("server_id1", o["server_id"]);
			Assert.AreEqual(@"c:\folder1\", o["backup_folder"]);
			Assert.AreEqual(123456L, o["backup_folder_free_space"]);
			Assert.AreEqual(0, o["photo_count"]);
			Assert.AreEqual(0, o["video_count"]);
			Assert.AreEqual(0, o["audio_count"]);

			Assert.IsTrue(ctx.GetState() is TransmitInitState);

			util.VerifyAll();
		}

		[TestMethod]
		public void testDisapprove()
		{
			var state = new WaitForApproveState();
			var ctx = new ProtocolContext(null, null, state) { device_id = "dd", device_name = "na" };

			string sentData = null;
			ctx.SendText = (t) => { sentData = t; };

			CloseStatusCode code = CloseStatusCode.NORMAL;
			string reason = null;
			ctx.Stop = (code1, reason1) => { code = code1; reason = reason1; };

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
