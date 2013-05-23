using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage;
using Moq;

namespace UnitTest.Pending
{
	[TestClass]
	public class testPendingToResource
	{
		PendingEvent evt;
		PendingToResource action;
		Mock<IPendingToResourceUtil> util;

		[TestInitialize]
		public void setup()
		{
			evt = new PendingEvent
			{
				files = new List<Guid>() { Guid.NewGuid() },
				time_start = new DateTime(2010, 10, 20, 12, 40, 50, DateTimeKind.Local),
				time_end = new DateTime(2010, 10, 22, 11, 22, 33, DateTimeKind.Local),
				title = "title",
				type = 1
			};


			util = new Mock<IPendingToResourceUtil>();
			util.SetReturnsDefault<List<PendingFileData>>(new List<PendingFileData>());
			util.SetReturnsDefault<string>(string.Empty);

			action = new PendingToResource(util.Object);
		}

		[TestMethod]
		public void use_year_month_title_as_folder()
		{
			util.Setup(x=>x.CreateFolder(@"title")).Verifiable();
			action.Do(evt);

			util.VerifyAll();
		}

		[TestMethod]
		public void use_station_generated_folder_if_no_title()
		{
			evt.title = null;
			util.Setup(x => x.CreateFolder(@"2010-10-20 12-40-50 ~ 2010-10-22 11-22-33")).Verifiable();
			action.Do(evt);

			util.VerifyAll();
		}

		[TestMethod]
		public void move_file_to_resource_folder()
		{
			util.Setup(x => x.CreateFolder(@"title")).Returns(@"title");
			util.Setup(x => x.GetPendingFolder()).Returns(".pending");
			util.Setup(x => x.GetPendingFiles(evt.files)).Returns(new List<PendingFileData> {
				new PendingFileData{ file_id = evt.files[0], file_name = "name.jpg", saved_path = "guid", dev_folder = "dev_folder"}
			});
			util.Setup(x => x.Move(@".pending\guid", @"dev_folder\title\name.jpg")).Returns(@"dev_folder\title\name.jpg").Verifiable();

			action.Do(evt);

			util.VerifyAll();
		}

		[TestMethod]
		public void move_record_from_pending_file_tables_to_files_table()
		{
			util.Setup(x => x.CreateFolder(@"title")).Returns(@"title");
			util.Setup(x => x.GetPendingFolder()).Returns(@"c:\bunny\.pending");
			util.Setup(x => x.GetResourceFolder()).Returns(@"c:\bunny");
			util.Setup(x => x.GetPendingFiles(evt.files)).Returns(new List<PendingFileData> {
				new PendingFileData{ file_id = evt.files[0], file_name = "name.jpg", saved_path = "guid", dev_folder = "dev_folder"}
			});
			util.Setup(x => x.Move(@"c:\bunny\.pending\guid", @"c:\bunny\dev_folder\title\name.jpg")).Returns(@"c:\bunny\dev_folder\title\name.1.jpg");

			List<FileData> records = null;
			util.Setup(x => x.MoveDbRecord(It.IsAny<List<FileData>>())).Callback<List<FileData>>((x) => { records = x; });

			action.Do(evt);

			Assert.AreEqual(evt.files[0], records[0].file_id);
			Assert.AreEqual(@"title", records[0].parent_folder);
			Assert.AreEqual(@"title\name.1.jpg", records[0].saved_path);
		}
	}
}
