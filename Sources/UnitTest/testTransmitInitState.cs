using InfiniteStorage;
using InfiniteStorage.Model;
using InfiniteStorage.WebsocketProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Newtonsoft.Json;

namespace UnitTest
{
	[TestClass]
	public class testTransmitInitState
	{
		Mock<IFileStorage> storage;
		Mock<ITempFile> tempFile;
		Mock<ITempFileFactory> fac;

		[TestInitialize]
		public void setup()
		{
			storage = new Mock<IFileStorage>();
			tempFile = new Mock<ITempFile>();
			fac = new Mock<ITempFileFactory>();
		}

		[TestMethod]
		public void testFileStart_no_dup()
		{
			fac.Setup(x => x.CreateTempFile()).Returns(tempFile.Object).Verifiable();

			var fileUtil = new Mock<IFileUtility>();
			var state = new TransmitInitState(fileUtil.Object);
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			string sentData = "";
			ctx.SendFunc = (x) => { sentData = x; };

			var cmd = new TextCommand
			{
				action = "file-start",
				file_name = "name",
				file_size = 1234,
				type = "audio",
				folder = "/sto/pp",
				datetime = DateTime.Now,
				total_count = 1000,
				backuped_count = 333,
			};

			ctx.handleFileStartCmd(cmd);

			Assert.AreEqual(cmd.file_name, ctx.fileCtx.file_name);
			Assert.AreEqual(cmd.file_size, ctx.fileCtx.file_size);
			Assert.AreEqual(FileAssetType.audio, ctx.fileCtx.type);
			Assert.AreEqual(cmd.folder, ctx.fileCtx.folder);
			Assert.AreEqual(cmd.datetime, ctx.fileCtx.datetime);
			Assert.AreEqual(cmd.total_count, ctx.total_count);
			Assert.AreEqual(cmd.backuped_count, ctx.backup_count);

			Assert.IsTrue(ctx.GetState() is TransmitStartedState);
		}

		[TestMethod]
		public void testFileStart_no_dup2()
		{
			fac.Setup(x => x.CreateTempFile()).Returns(tempFile.Object).Verifiable();

			var fileUtil = new Mock<IFileUtility>();
			var state = new TransmitInitState(fileUtil.Object);
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			string sentData = "";
			ctx.SendFunc = (x) => { sentData = x; };

			var cmd = new TextCommand
			{
				action = "file-start",
				file_name = "name",
				file_size = 1234,
				type = "audio",
				folder = "/sto/pp",
				datetime = DateTime.Now,
				total_count = 1000,
				backuped_count = 333,
			};

			ctx.handleFileStartCmd(cmd);

			var o = JsonConvert.DeserializeObject<TextCommand>(sentData);
			Assert.AreEqual("file-go", o.action);
			Assert.AreEqual("name", o.file_name);

		}

		[TestMethod]
		public void testFileStart_has_dup()
		{
			fac.Setup(x => x.CreateTempFile()).Returns(tempFile.Object).Verifiable();

			var fileUtil = new Mock<IFileUtility>();
			fileUtil.Setup(x => x.HasDuplicateFile(It.IsAny<FileContext>(), It.IsAny<string>())).Returns(true);
			var state = new TransmitInitState(fileUtil.Object);
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			string sentData = "";
			ctx.SendFunc = (x) => { sentData = x; };

			var cmd = new TextCommand
			{
				action = "file-start",
				file_name = "name",
				file_size = 1234,
				type = "audio",
				folder = "/sto/pp",
				datetime = DateTime.Now,
				total_count = 1000,
				backuped_count = 333,
			};

			ctx.handleFileStartCmd(cmd);

			Assert.IsTrue(ctx.GetState() is TransmitInitState);
			Assert.IsNull(ctx.fileCtx);

			var o = JsonConvert.DeserializeObject<TextCommand>(sentData);
			Assert.AreEqual("file-exist", o.action);
			Assert.AreEqual("name", o.file_name);
		}

		[TestMethod]
		[ExpectedException(typeof(ProtocolErrorException))]
		public void no_type_error()
		{

			var state = new TransmitInitState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			var cmd = new TextCommand
			{
				action = "file-start",
				file_name = "name",
				file_size = 1234,
				folder = "/sto/pp",
				datetime = DateTime.Now
			};

			ctx.handleFileStartCmd(cmd);
		}

		[TestMethod]
		[ExpectedException(typeof(ProtocolErrorException))]
		public void unknown_type_error()
		{

			var state = new TransmitInitState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			var cmd = new TextCommand
			{
				action = "file-start",
				file_name = "name",
				file_size = 1234,
				type = "~~~~~~~ooxx123",
				folder = "/sto/pp",
				datetime = DateTime.Now
			};

			ctx.handleFileStartCmd(cmd);
		}

		[TestMethod]
		public void ctx_counts_are_updated_by_update_count_msg() // because update-count is deprecated
		{
			var state = new TransmitInitState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			ctx.total_count = 1000;

			var cmd = new TextCommand
			{
				action = "update-count",
				transfer_count = 3322,
				backuped_count = 1000,
			};

			ctx.handleUpdateCountCmd(cmd);

			Assert.AreEqual(cmd.transfer_count, ctx.total_count);
			Assert.AreEqual(cmd.backuped_count, ctx.backup_count);
			Assert.AreEqual(state, ctx.GetState());
		}
	}
}
