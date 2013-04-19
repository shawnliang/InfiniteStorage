using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage;
using InfiniteStorage.Model;
using Moq;

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
		public void testFileStart()
		{
			fac.Setup(x=>x.CreateTempFile()).Returns(tempFile.Object).Verifiable();

			var state = new TransmitInitState();
			var ctx =new ProtocolContext(fac.Object, storage.Object, state);
			var cmd = new TextCommand
			{
				action = "file-start",
				file_name = "name",
				file_size = 1234,
				type = "audio",
				folder = "/sto/pp",
				datetime = DateTime.Now
			};
			
			ctx.handleFileStartCmd(cmd);

			Assert.AreEqual(cmd.file_name, ctx.fileCtx.file_name);
			Assert.AreEqual(cmd.file_size, ctx.fileCtx.file_size);
			Assert.AreEqual(FileAssetType.audio, ctx.fileCtx.type);
			Assert.AreEqual(cmd.folder, ctx.fileCtx.folder);
			Assert.AreEqual(cmd.datetime, ctx.fileCtx.datetime);


			Assert.IsTrue(ctx.GetState() is TransmitStartedState);
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
				type ="~~~~~~~ooxx123",
				folder = "/sto/pp",
				datetime = DateTime.Now
			};

			ctx.handleFileStartCmd(cmd);
		}
	}
}
