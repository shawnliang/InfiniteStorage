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
		[TestMethod]
		public void imageMimeTypesAreSavedToPhotoLocation()
		{
			var fileCtx = new FileContext { file_name = "a.jpg", mimetype = "image/jpeg", datetime = new DateTime(2010, 10, 12) };

			var fileMover = new Mock<IFileMove>();
			fileMover.Setup(x => x.Move("temp.jpg", @"p\dev\xxxxxx\yyyyyy\a.jpg")).Verifiable();


			var dirOrg = new Mock<IDirOrganizer>();
			dirOrg.Setup(x => x.GetDir(fileCtx)).Returns(@"xxxxxx\yyyyyy").Verifiable();

			var stor = new FlatFileStorage("p", "v", "a", dirOrg.Object);
			stor.FileMover = fileMover.Object;

			stor.setDeviceName("dev");
			stor.MoveToStorage("temp.jpg", fileCtx);

			dirOrg.VerifyAll();
			fileMover.VerifyAll();
		}

		[TestMethod]
		public void videoMimeTypesAreSavedToPhotoLocation()
		{
			var fileCtx = new FileContext { file_name = "a.jpg", mimetype = "video/jpeg", datetime = new DateTime(2010, 10, 12) };

			var fileMover = new Mock<IFileMove>();
			fileMover.Setup(x => x.Move("temp.jpg", @"v\dev\xxxxxx\yyyyyy\a.jpg")).Verifiable();


			var dirOrg = new Mock<IDirOrganizer>();
			dirOrg.Setup(x => x.GetDir(fileCtx)).Returns(@"xxxxxx\yyyyyy").Verifiable();

			var stor = new FlatFileStorage("p", "v", "a", dirOrg.Object);
			stor.FileMover = fileMover.Object;

			stor.setDeviceName("dev");
			stor.MoveToStorage("temp.jpg", fileCtx);

			dirOrg.VerifyAll();
			fileMover.VerifyAll();
		}
	}
}
