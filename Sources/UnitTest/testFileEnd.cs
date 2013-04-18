using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage.WebsocketProtocol;
using Moq;
using InfiniteStorage;
using InfiniteStorage.Model;
using System.IO;

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
				fileCtx = new FileContext() { file_name = "n", file_size = 1000, folder = "f", mimetype = "mm", UTI = "uu", datetime = DateTime.Now },
				temp_file = temp.Object
			};
		}


		[TestMethod]
		public void recv_file_happy_case()
		{
			temp.Setup(x => x.EndWrite()).Verifiable();
			temp.Setup(x=>x.Path).Returns("path1").Verifiable();
			storage.Setup(x => x.MoveToStorage("path1", ctx.fileCtx)).Verifiable();
			util.Setup(x => x.SaveFileRecord(It.Is<FileAsset>(
				f => 
					f.device_id == ctx.device_id &&
					f.event_time == ctx.fileCtx.datetime &&
					!f.file_id.Equals(Guid.Empty) &&
					f.file_name == ctx.fileCtx.file_name &&
					f.file_path == Path.Combine(ctx.fileCtx.folder, ctx.fileCtx.file_name) &&
					f.file_size == ctx.fileCtx.file_size &&
					f.mime_type == ctx.fileCtx.mimetype &&
					f.uti == ctx.fileCtx.UTI)
					
				)).Verifiable();


			var state = new TransmitStartedState() { Util = util.Object };
			
			ctx.SetState(state);
			ctx.handleFileEndCmd(new TextCommand { action = "file-end", file_name = "f.jpg" });

			Assert.AreEqual(1, ctx.recved_files);
			Assert.IsTrue(ctx.GetState() is TransmitInitState);

			util.VerifyAll();
			temp.VerifyAll();
			storage.VerifyAll();
		}

		[TestMethod]
		public void recv_duplicate_file()
		{
			temp.Setup(x => x.EndWrite()).Verifiable();
			temp.Setup(x => x.Delete()).Verifiable();
			util.Setup(x => x.HasDuplicateFile(ctx.fileCtx, ctx.device_id)).Returns(true).Verifiable();

			var state = new TransmitStartedState() { Util = util.Object };

			ctx.SetState(state);
			ctx.handleFileEndCmd(new TextCommand { action = "file-end", file_name = "f.jpg" });

			Assert.AreEqual(1, ctx.recved_files);
			Assert.IsTrue(ctx.GetState() is TransmitInitState);

			temp.VerifyAll();
		}
	}
}
