using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage;
using Moq;
using WebSocketSharp;
using Newtonsoft.Json;
using InfiniteStorage.WebsocketProtocol;

namespace UnitTest
{
	[TestClass]
	public class testProtocolHandler
	{
		Mock<IFileStorage> storage;

		[TestInitialize]
		public void setup()
		{
			storage = new Mock<IFileStorage>();
		}

		[TestMethod]
		public void testNormalRecv()
		{
			var data1 = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
			var data2 = new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

			var tempFile = new Mock<ITempFile>();
			tempFile.Setup(x => x.Write(data1)).Verifiable();
			tempFile.Setup(x => x.Write(data2)).Verifiable();
			tempFile.Setup(x => x.EndWrite()).Verifiable();
			tempFile.Setup(x=>x.Path).Returns("path").Verifiable();

			var tempFactory = new Mock<ITempFileFactory>();
			tempFactory.Setup(x => x.CreateTempFile()).Returns(tempFile.Object).Verifiable();

			storage.Setup(x => x.MoveToStorage("path", "file1.jpg")).Verifiable();

			var protoHdler = new ProtocolHanlder(tempFactory.Object, storage.Object);
			
			
			//
			// file-start
			// 
			var fileStart = new { action = "file-start", file_name = "file1.jpg", file_size= 20 };
			protoHdler.HandleMessage(new MessageEventArgs(JsonConvert.SerializeObject(fileStart)));


			//
			// 2 data frames
			// 		
			protoHdler.HandleMessage(new MessageEventArgs(WebSocketSharp.Frame.Opcode.BINARY,
				new WebSocketSharp.Frame.PayloadData(data1)));
			protoHdler.HandleMessage(new MessageEventArgs(WebSocketSharp.Frame.Opcode.BINARY,
				new WebSocketSharp.Frame.PayloadData(data2)));

			//
			// file-end
			//
			var fileEnd = new { action = "file-end", file_name = fileStart.file_name };
			protoHdler.HandleMessage(new MessageEventArgs(JsonConvert.SerializeObject(fileEnd)));


			// ---------- verify --------------
			tempFactory.VerifyAll();
			tempFile.VerifyAll();
			storage.VerifyAll();
			
		}

		[TestMethod]
		[ExpectedException(typeof(ProtocolErrorException))]
		public void testUnknownTxtCmd()
		{
			var tempFactory = new Mock<ITempFileFactory>();
			var protoHdler = new ProtocolHanlder(tempFactory.Object, storage.Object);

			//
			// Unknown command XXXXX
			// 
			var fileStart = new { action = "XXXXX", file_name = "file1.jpg", file_size = 20 };
			protoHdler.HandleMessage(new MessageEventArgs(JsonConvert.SerializeObject(fileStart)));

		}

		[TestMethod]
		public void tempFileIsDeletedIfErrorHappens()
		{
			var tempFile = new Mock<ITempFile>();
			tempFile.Setup(x => x.Delete()).Verifiable();

			var tempFactory = new Mock<ITempFileFactory>();
			tempFactory.Setup(x => x.CreateTempFile()).Returns(tempFile.Object).Verifiable();

			var protoHdler = new ProtocolHanlder(tempFactory.Object, storage.Object);


			//
			// file-start
			// 
			var fileStart = new { action = "file-start", file_name = "file1.jpg", file_size = 20 };
			protoHdler.HandleMessage(new MessageEventArgs(JsonConvert.SerializeObject(fileStart)));

			// file-start again !!! Error
			try
			{
				protoHdler.HandleMessage(new MessageEventArgs(JsonConvert.SerializeObject(fileStart)));

				Assert.Fail("ProtocolErrorException is expected but is not thrown");
			}
			catch (ProtocolErrorException)
			{
				// ======= verify =======
				tempFile.VerifyAll();
			}
		}
	}
}
