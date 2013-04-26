using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage;
using Moq;
using InfiniteStorage.WebsocketProtocol;

namespace UnitTest
{
	[TestClass]
	public class testFileStorage
	{
		Mock<IFileStorageLocationProvider> loc;

		[TestInitialize]
		public void setup()
		{
			loc = new Mock<IFileStorageLocationProvider>();
			loc.Setup(x => x.AudioLocation).Returns("aa");
			loc.Setup(x => x.VideoLocation).Returns("vv");
			loc.Setup(x => x.PhotoLocation).Returns("pp");
		}

		[TestMethod]
		public void imageMimeTypesAreSavedToPhotoLocation()
		{
			var fileCtx = new FileContext { file_name = "a.jpg", type= InfiniteStorage.Model.FileAssetType.image, datetime = new DateTime(2010, 10, 12) };

			var fileMover = new Mock<IFileMove>();
			fileMover.Setup(x => x.Move("temp.jpg", @"pp\dev\xxxxxx\yyyyyy\a.jpg")).Returns(@"pp\dev\xxxxxx\yyyyyy\a.jpg").Verifiable();


			var dirOrg = new Mock<IDirOrganizer>();
			dirOrg.Setup(x => x.GetDir(fileCtx)).Returns(@"xxxxxx\yyyyyy").Verifiable();

			var stor = new FlatFileStorage(dirOrg.Object);
			stor.FileMover = fileMover.Object;

			stor.StorageLocationProvider = loc.Object;
			stor.setDeviceName("dev");
			var saved = stor.MoveToStorage("temp.jpg", fileCtx);

			dirOrg.VerifyAll();
			fileMover.VerifyAll();


			Assert.AreEqual(@"pp\dev", saved.device_folder);
			Assert.AreEqual(@"xxxxxx\yyyyyy\a.jpg", saved.relative_file_path);
		}

		[TestMethod]
		public void videoMimeTypesAreSavedToVideoLocation()
		{
			var fileCtx = new FileContext { file_name = "a.jpg", type = InfiniteStorage.Model.FileAssetType.video, datetime = new DateTime(2010, 10, 12) };

			var fileMover = new Mock<IFileMove>();
			fileMover.Setup(x => x.Move("temp.jpg", @"vv\dev\xxxxxx\yyyyyy\a.jpg")).Returns(@"vv\dev\xxxxxx\yyyyyy\a.jpg").Verifiable();


			var dirOrg = new Mock<IDirOrganizer>();
			dirOrg.Setup(x => x.GetDir(fileCtx)).Returns(@"xxxxxx\yyyyyy").Verifiable();

			var stor = new FlatFileStorage(dirOrg.Object);
			stor.FileMover = fileMover.Object;
			
			stor.StorageLocationProvider = loc.Object;
			stor.setDeviceName("dev");
			stor.MoveToStorage("temp.jpg", fileCtx);

			dirOrg.VerifyAll();
			fileMover.VerifyAll();
		}
	}
}
