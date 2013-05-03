using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InfiniteStorage.Model;

namespace UnitTest
{
	[TestClass]
	public class testConnectMsgHandler
	{
		ProtocolContext ctx = null;
		Mock<IFileStorage> fileStorage;
		
		[TestInitialize]
		public void setup()
		{
			var factory = new Mock<ITempFileFactory>();
			var state = new Mock<AbstractProtocolState>();
			fileStorage = new Mock<IFileStorage>();

			ctx = new ProtocolContext(factory.Object, fileStorage.Object, state.Object);
		}

		[TestMethod]
		public void replyAcceptMsgData_HaveExistingData()
		{
			fileStorage.Setup(x => x.setDeviceName("dev123")).Verifiable();
			string sentTxt = null;
			ctx.SendFunc = (txt) => sentTxt = txt;

			ProtocolContext evtCtx = null;
			ctx.OnConnectAccepted += (sr, ev) => { evtCtx = ev.ctx; };

			ctx.device_name = "dev";

			var util = new Mock<IConnectMsgHandlerUtil>();
			util.Setup(x => x.GetClientInfo("id1")).Returns(new Device
			{
				device_id = "id1",
				device_name = "dev",
				folder_name = "dev123"
			});
			util.Setup(x => x.GetServerId()).Returns("server_id1");
			util.Setup(x => x.GetPhotoFolder()).Returns(@"c:\folder1\");
			util.Setup(x => x.GetFreeSpace(@"c:\folder1\")).Returns(102410241024L);
			util.Setup(x => x.GetDeviceSummary("id1")).Returns(
				new DeviceSummary
				{
					photo_count = 100,
					video_count = 200,
					audio_count = 300,
					backup_range = new TimeRange(
								 new DateTime(2012, 1, 2, 0, 0, 0, DateTimeKind.Utc).ToLocalTime(),
								 new DateTime(2012, 1, 3, 0, 0, 0, DateTimeKind.Utc))
				}).Verifiable();

			var hdl = new ConnectMsgHandler();
			hdl.Util = util.Object;
			hdl.HandleConnectMsg(new TextCommand { action = "connect", device_name = "dev", device_id = "id1", transfer_count = 111 }, ctx);

			JObject o = JObject.Parse(sentTxt);
			Assert.AreEqual("accept", o["action"]);
			Assert.AreEqual("server_id1", o["server_id"]);
			Assert.AreEqual(@"c:\folder1\", o["backup_folder"]);
			Assert.AreEqual(102410241024L, o["backup_folder_free_space"]);
			Assert.AreEqual(100, o["photo_count"]);
			Assert.AreEqual(200, o["video_count"]);
			Assert.AreEqual(300, o["audio_count"]);
			Assert.AreEqual(new DateTime(2012, 1, 2, 0, 0, 0, DateTimeKind.Utc), o["backup_startdate"].Value<DateTime>());
			Assert.AreEqual(new DateTime(2012, 1, 3, 0, 0, 0, DateTimeKind.Utc), o["backup_enddate"]);


			Assert.AreEqual(ctx, evtCtx);
			Assert.IsTrue(ctx.GetState() is TransmitInitState);

			fileStorage.VerifyAll();
		}

		[TestMethod]
		public void reject_all_unpaired_devices()
		{
			string sentData = null;
			ctx.SendFunc = (data) => { sentData = data; };

			WebSocketSharp.Frame.CloseStatusCode opcode = WebSocketSharp.Frame.CloseStatusCode.NORMAL;
			string reason = "";
			ctx.StopFunc = (op1, re1) => { opcode = op1; reason = re1; };

			var util = new Mock<IConnectMsgHandlerUtil>();
			util.Setup(x => x.GetClientInfo("id1"));
			util.Setup(x => x.RejectUnpairedDevices).Returns(true).Verifiable();

			var handler = new ConnectMsgHandler() { Util = util.Object };

			handler.HandleConnectMsg(
				new TextCommand {
					action = "connect",
					device_name = "dev",
					device_id = "id1",
					transfer_count = 111
				}, ctx);

			var o = JObject.Parse(sentData);
			Assert.AreEqual("denied", o["action"]);
			Assert.AreEqual("Not allowed", o["reason"]);

			Assert.AreEqual(WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, opcode);
			Assert.AreEqual("Not allowed", reason);

			util.VerifyAll();

			Assert.IsTrue(ctx.GetState() is UnconnectedState);
		}

		[TestMethod]
		public void firstConnect__replyWaitForPairing()
		{
			string sentTxt = null;
			ctx.SendFunc = (txt) => sentTxt = txt;

			ProtocolContext evtCtx = null;
			ctx.OnPairingRequired += (sr, ev) => { evtCtx = ev.ctx; };

			var util = new Mock<IConnectMsgHandlerUtil>();
			util.Setup(x => x.GetClientInfo("id1")).Returns(null as Device);

			var hdl = new ConnectMsgHandler();
			hdl.Util = util.Object;
			hdl.HandleConnectMsg(new TextCommand { action = "connect", device_name = "dev", device_id = "id1", transfer_count = 111 }, ctx);


			Assert.IsFalse(string.IsNullOrEmpty(sentTxt));
			var o = JObject.Parse(sentTxt);
			Assert.AreEqual("wait-for-pair", o["action"]);

			Assert.AreEqual(evtCtx, ctx);
			Assert.IsTrue(ctx.GetState() is WaitForApproveState);
		}
	}
}

