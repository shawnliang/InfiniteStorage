﻿using InfiniteStorage.Notify;
using InfiniteStorage.WebsocketProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

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
		public void happyCase_files()
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
			Assert.AreEqual(true, ctx.subscribe_files);
			Assert.AreEqual(false, ctx.subscribe_labels);
			Assert.AreEqual(false, ctx.subscribe_devices);
		}

		[TestMethod]
		public void happyCase_labels()
		{
			var msg = JsonConvert.SerializeObject(new
			{
				connect = new { device_id = "dev", device_name = "name" },
				subscribe = new { labels = true, labels_from_seq = 1023 }
			});


			SubscriptionContext ctx = null;
			handler.Subscribing += (s, e) => { ctx = e.Ctx; };
			handler.HandleMessage(svc.Object, new WebSocketSharp.MessageEventArgs(msg));

			Assert.AreEqual("name", ctx.device_name);
			Assert.AreEqual("dev", ctx.device_id);
			Assert.AreEqual(false, ctx.subscribe_files);
			Assert.AreEqual(true, ctx.subscribe_labels);
			Assert.AreEqual(1023, ctx.labels_from_seq);
			Assert.AreEqual(false, ctx.subscribe_devices);
		}

		[TestMethod]
		public void happy_case_device()
		{
			var msg = JsonConvert.SerializeObject(new
			{
				connect = new { device_id = "dev", device_name = "name" },
				subscribe = new { devices = true }
			});

			SubscriptionContext ctx = null;
			handler.Subscribing += (s, e) => { ctx = e.Ctx; };
			handler.HandleMessage(svc.Object, new WebSocketSharp.MessageEventArgs(msg));

			Assert.AreEqual("name", ctx.device_name);
			Assert.AreEqual("dev", ctx.device_id);
			Assert.AreEqual(false, ctx.subscribe_files);
			Assert.AreEqual(false, ctx.subscribe_labels);
			Assert.AreEqual(true, ctx.subscribe_devices);
		}

		[TestMethod]
		public void happyCase_files_and_labels()
		{
			var msg = JsonConvert.SerializeObject(new
			{
				connect = new { device_id = "dev", device_name = "name" },
				subscribe = new { labels = true, files_from_seq = 1234 }
			});


			SubscriptionContext ctx = null;
			handler.Subscribing += (s, e) => { ctx = e.Ctx; };
			handler.HandleMessage(svc.Object, new WebSocketSharp.MessageEventArgs(msg));

			Assert.AreEqual("name", ctx.device_name);
			Assert.AreEqual("dev", ctx.device_id);
			Assert.AreEqual(true, ctx.subscribe_files);
			Assert.AreEqual(true, ctx.subscribe_labels);
			Assert.AreEqual(1234, ctx.files_from_seq);
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
