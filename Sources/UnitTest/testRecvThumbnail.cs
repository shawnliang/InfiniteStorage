using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage;
using Moq;

namespace UnitTest
{
	[TestClass]
	public class testRecvThumbnail
	{
		Mock<ITempFileFactory> fac;
		Mock<IFileStorage> storage;
		Mock<ITempFile> tempFile;

		[TestInitialize]
		public void setup()
		{
			fac = new Mock<ITempFileFactory>();
			storage = new Mock<IFileStorage>();
			tempFile = new Mock<ITempFile>();

			fac.Setup(x => x.CreateTempFile()).Returns(tempFile.Object);
		}

		[TestMethod]
		public void GoToThumbnailStartedState_WaitForPairingState_thumbnailStart()
		{
			var state = new WaitForApproveState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			var cmd = new TextCommand { action = "thumbnail-start", transfer_count = 50 };
			
			state.handleThumbStartCmd(ctx, cmd);

			Assert.IsTrue(ctx.GetState() is ThumbnailStartedState);
		}

		[TestMethod]
		public void ThumbnailIsCreated_WaitForPairingState_thumbnailStart()
		{
			var state = new WaitForApproveState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			var cmd = new TextCommand { action = "thumbnail-start", transfer_count = 50 };

			state.handleThumbStartCmd(ctx, cmd);

			var temp = ctx.GetData(ThumbnailStartedState.TEMP_FILE_KEY);
			Assert.IsTrue(temp is ITempFile);
		}

		[TestMethod]
		public void ThumbnailDataIsWrittenToTempFile_ThumbnailStartedState_BinaryData()
		{
			var data = new byte[] { 1, 2, 3 };
			var state = new ThumbnailStartedState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);

			tempFile.Setup(x=>x.Write(data)).Verifiable();
			ctx.SetData(ThumbnailStartedState.TEMP_FILE_KEY, tempFile.Object);


			state.handleBinaryData(ctx, data);
			tempFile.VerifyAll();
		}

		[TestMethod]
		public void ThumbnailTempIsFinalized_ThumbnailStartedState_ThumbnailEnd()
		{
			var state = new ThumbnailStartedState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			tempFile.Setup(x => x.EndWrite()).Verifiable();
			ctx.SetData(ThumbnailStartedState.TEMP_FILE_KEY, tempFile.Object);


			state.handleThumbEndCmd(ctx, new TextCommand { action = "thumbnail-end" });
			tempFile.VerifyAll();
		}

		[TestMethod]
		public void GoToWaitForPairingState_ThumbnailStartedState_ThumbnailEnd()
		{
			var state = new ThumbnailStartedState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			tempFile.Setup(x => x.EndWrite()).Verifiable();
			ctx.SetData(ThumbnailStartedState.TEMP_FILE_KEY, tempFile.Object);

			state.handleThumbEndCmd(ctx, new TextCommand { action = "thumbnail-end" });

			Assert.IsTrue(ctx.GetState() is WaitForApproveState);
		}

		[TestMethod]
		public void ThumbnailRecvEventIsRaised_ThumbnailStartedState_ThumbnailEnd()
		{
			var state = new ThumbnailStartedState();
			var ctx = new ProtocolContext(fac.Object, storage.Object, state);
			tempFile.Setup(x => x.EndWrite()).Verifiable();
			ctx.SetData(ThumbnailStartedState.TEMP_FILE_KEY, tempFile.Object);

			bool eventRaised = false;
			ctx.OnThumbnailReceived += (s, e) => { eventRaised = true; };


			state.handleThumbEndCmd(ctx, new TextCommand { action = "thumbnail-end" });

			Assert.IsTrue(eventRaised);
		}
	}
}
