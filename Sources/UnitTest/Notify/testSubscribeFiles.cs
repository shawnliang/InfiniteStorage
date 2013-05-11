using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using InfiniteStorage.Notify;
using Newtonsoft.Json;
using InfiniteStorage.WebsocketProtocol;

namespace UnitTest.Notify
{
	[TestClass]
	public class testSubscribeFiles
	{
		private NotifyChannelHandler handler;
		private Mock<WebSocketSharp.Server.WebSocketService> svc;

		[TestInitialize]
		public void Setup()
		{
			handler = new NotifyChannelHandler();
			svc = new Mock<WebSocketSharp.Server.WebSocketService>();
		}

		[TestMethod]
		public void happyCase()
		{
			var msg = JsonConvert.SerializeObject(new
			{
				connect = new { device_id = "dev", device_name = "name" },
				subscribe = new { files_from_seq = 1000 }
			});


			SubscriptionContext ctx = null;
			handler.Subscribing += (s, e) => { ctx = e.Ctx; };
			handler.HandleMessage(svc.Object, new WebSocketSharp.MessageEventArgs(msg));


			Assert.AreEqual(1000, ctx.files_from_seq);
			Assert.AreEqual("name", ctx.device_name);
			Assert.AreEqual("dev", ctx.device_id);
		}

		[TestMethod]
		[ExpectedException(typeof(ProtocolErrorException))]
		public void no_connect()
		{
			var msg = JsonConvert.SerializeObject(new
			{
				subscribe = new { files_from_seq = 1000 }
			});

			handler.HandleMessage(svc.Object, new WebSocketSharp.MessageEventArgs(msg));
		}

		[TestMethod]
		[ExpectedException(typeof(ProtocolErrorException))]
		public void no_subscribe()
		{
			var msg = JsonConvert.SerializeObject(new
			{
				connect = new { device_id = "id", device_name = "name" }
			});

			handler.HandleMessage(svc.Object, new WebSocketSharp.MessageEventArgs(msg));
		}

		[TestMethod]
		[ExpectedException(typeof(ProtocolErrorException))]
		public void connect_twice()
		{
			handler.Ctx = new SubscriptionContext("id", "name");

			var msg = JsonConvert.SerializeObject(new
			{
				connect = new { device_id = "dev", device_name = "name" },
				subscribe = new { files_from_seq = 1000 }
			});

			handler.HandleMessage(svc.Object, new WebSocketSharp.MessageEventArgs(msg));
		}
	}
}
