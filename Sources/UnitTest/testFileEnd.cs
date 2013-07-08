using InfiniteStorage;
using InfiniteStorage.Model;
using InfiniteStorage.WebsocketProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using Newtonsoft.Json;

namespace UnitTest
{
	[TestClass]
	public class testFileEnd
	{
		Mock<ITempFileFactory> fac;
		Mock<ITempFile> temp;
		Mock<IFileStorage> storage;
		Mock<ITransmitStateUtility> util;
		ProtocolContext ctx;

		[TestInitialize]
		public void setup()
		{
			fac = new Mock<ITempFileFactory>();
			temp = new Mock<ITempFile>();
			storage = new Mock<IFileStorage>();
			util = new Mock<ITransmitStateUtility>();

			ctx = new ProtocolContext(fac.Object, storage.Object, null)
			{
				device_id = "dev_id",
				fileCtx = new FileContext() { file_name = "n", file_size = 1000, folder = "f", datetime = DateTime.Now, type = FileAssetType.image },
				temp_file = temp.Object
			};
		}


		[TestMethod]
		public void recv_file_happy_case()
		{
			//var saved = new SavedPath { device_folder = "fff", relative_file_path = "rrr" };

			temp.Setup(x => x.EndWrite()).Verifiable();
			temp.Setup(x => x.Path).Returns("path1").Verifiable();
			storage.Setup(x => x.MoveToStorage("path1", ctx.fileCtx)).Returns(@"fff\rrr").Verifiable();
			util.Setup(x => x.GetNextSeq()).Returns(112345).Verifiable();
			util.Setup(x => x.SaveFileRecord(It.Is<FileAsset>(
				f =>
					f.device_id == ctx.device_id &&
					f.event_time == ctx.fileCtx.datetime &&
					!f.file_id.Equals(Guid.Empty) &&
					f.file_name == ctx.fileCtx.file_name &&
					f.file_path == Path.Combine(ctx.fileCtx.folder, ctx.fileCtx.file_name) &&
					f.file_size == ctx.fileCtx.file_size &&
					f.type == (int)ctx.fileCtx.type &&
					f.saved_path == @"fff\rrr" &&
					f.parent_folder == "fff" &&
					f.seq == 112345)
				)).Verifiable();


			var state = new TransmitStartedState() { Util = util.Object };

			ctx.SetState(state);
			string sentData = "";
			ctx.SendFunc = (x) => { sentData = x; };
			bool called = false;
			ctx.OnFileReceived += (s, e) => { called = true; };
			ctx.handleFileEndCmd(new TextCommand { action = "file-end", file_name = "f.jpg" });

			Assert.AreEqual(1, ctx.recved_files);
			Assert.IsTrue(ctx.GetState() is TransmitInitState);

			util.VerifyAll();
			temp.VerifyAll();
			storage.VerifyAll();
			Assert.IsTrue(called);

			var o = JsonConvert.DeserializeObject<TextCommand>(sentData);
			Assert.AreEqual("file-exist", o.action);
			Assert.AreEqual(ctx.fileCtx.file_name, o.file_name);
		}

		[TestMethod]
		public void recv_duplicate_file()
		{
			temp.Setup(x => x.EndWrite()).Verifiable();
			temp.Setup(x => x.Delete()).Verifiable();
			util.Setup(x => x.HasDuplicateFile(ctx.fileCtx, ctx.device_id)).Returns(true).Verifiable();

			var state = new TransmitStartedState() { Util = util.Object };

			string sentData = "";
			ctx.SendFunc = (x) => { sentData = x; };
			ctx.SetState(state);
			ctx.handleFileEndCmd(new TextCommand { action = "file-end", file_name = "f.jpg" });

			Assert.AreEqual(1, ctx.recved_files);
			Assert.IsTrue(ctx.GetState() is TransmitInitState);

			temp.VerifyAll();


			var o = JsonConvert.DeserializeObject<TextCommand>(sentData);
			Assert.AreEqual("file-exist", o.action);
			Assert.AreEqual(ctx.fileCtx.file_name, o.file_name);
		}
	}
}
