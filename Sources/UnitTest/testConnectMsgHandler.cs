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
		
		
		[TestInitialize]
		public void setup()
		{
			var factory = new Mock<ITempFileFactory>();
			var fileStorage = new Mock<IFileStorage>();
			var state = new Mock<AbstractProtocolState>();

			ctx = new ProtocolContext(factory.Object, fileStorage.Object, state.Object);
		}

		[TestMethod]
		public void replyAcceptMsgData_HaveExistingData()
		{
			string sentTxt = null;
			ctx.SendText = (txt) => sentTxt = txt;

			ProtocolContext evtCtx = null;
			ctx.OnConnectAccepted += (sr, ev) => { evtCtx = ev.ctx; };

			var util = new Mock<IConnectMsgHandlerUtil>();
			util.Setup(x => x.GetClientInfo("id1")).Returns(new Device
			{
				device_id = "id1",
				device_name = "dev",
				photo_count = 100,
				video_count = 200,
				audio_count = 300
			});
			util.Setup(x => x.GetServerId()).Returns("server_id1");
			util.Setup(x => x.GetPhotoFolder()).Returns(@"c:\folder1\");
			util.Setup(x => x.GetFreeSpace(@"c:\folder1\")).Returns(102410241024L);

			var hdl = new ConnectMsgHandler();
			hdl.Util = util.Object;
			var newState = hdl.HandleConnectMsg(new TextCommand { action = "connect", device_name = "dev", device_id = "id1", transfer_count = 111 }, ctx);

			JObject o = JObject.Parse(sentTxt);
			Assert.AreEqual("accept", o["action"]);
			Assert.AreEqual("server_id1", o["server_id"]);
			Assert.AreEqual(@"c:\folder1\", o["backup_folder"]);
			Assert.AreEqual(102410241024L, o["backup_folder_free_space"]);
			Assert.AreEqual(100, o["photo_count"]);
			Assert.AreEqual(200, o["video_count"]);
			Assert.AreEqual(300, o["audio_count"]);

			Assert.AreEqual(ctx, evtCtx);
			Assert.IsTrue(newState is TransmitInitState);
		}


		[TestMethod]
		public void replyAcceptMsgData_HaveNoData()
		{
			string sentTxt = null;
			ctx.SendText = (txt) => sentTxt = txt;

			ProtocolContext evtCtx = null;
			ctx.OnConnectAccepted += (sr, ev) => { evtCtx = ev.ctx; };

			var util = new Mock<IConnectMsgHandlerUtil>();
			util.Setup(x => x.GetClientInfo("id1")).Returns(null as Device);
			util.Setup(x => x.GetServerId()).Returns("server_id1");
			util.Setup(x => x.GetPhotoFolder()).Returns(@"c:\folder1\");
			util.Setup(x => x.GetFreeSpace(@"c:\folder1\")).Returns(102410241024L);
			util.Setup(x => x.Save(It.Is<Device>((y)=>
				y.audio_count == 0 && y.device_id == "id1" && y.device_name == "dev" && y.photo_count == 0 && y.video_count == 0 
				))).Verifiable();

			var hdl = new ConnectMsgHandler();
			hdl.Util = util.Object;
			var newState = hdl.HandleConnectMsg(new TextCommand { action = "connect", device_name = "dev", device_id = "id1", transfer_count = 111 }, ctx);

			JObject o = JObject.Parse(sentTxt);
			Assert.AreEqual("accept", o["action"]);
			Assert.AreEqual("server_id1", o["server_id"]);
			Assert.AreEqual(@"c:\folder1\", o["backup_folder"]);
			Assert.AreEqual(102410241024L, o["backup_folder_free_space"]);
			Assert.AreEqual(0, o["photo_count"]);
			Assert.AreEqual(0, o["video_count"]);
			Assert.AreEqual(0, o["audio_count"]);

			Assert.AreEqual(evtCtx, ctx);
			Assert.IsTrue(newState is TransmitInitState);
		}
	}
}

