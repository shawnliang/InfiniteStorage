using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage.Model;
using InfiniteStorage.Share;
using Moq;


namespace UnitTest.share
{
	[TestClass]
	public class testEnableTask
	{
		ShareEnableTask task;
		Mock<IShareEnableTaskDB> db;
		Mock<IShareCloudAPI> api;


		[TestInitialize]
		public void setup()
		{
			db = new Mock<IShareEnableTaskDB>();
			db.SetReturnsDefault<ICollection<FileAsset>>(new List<FileAsset>());
			db.SetReturnsDefault<ICollection<ShareRecipient>>(new List<ShareRecipient>());

			api = new Mock<IShareCloudAPI>();

			task = new ShareEnableTask(db.Object, api.Object);
		}

		[TestMethod]
		public void upload_not_on_cloud_file_assets()
		{
			var label = new Label{ label_id = Guid.NewGuid(), seq = 500, share_proc_seq = 100 };

			var file1 = new FileAsset { file_id = Guid.NewGuid() };
			var file2 = new FileAsset { file_id = Guid.NewGuid(), on_cloud = false };
			var file_already_on_cloud = new FileAsset { file_id = Guid.NewGuid(), on_cloud = true };

			db.Setup(x => x.QueryLabelFiles(label)).Returns(new List<FileAsset> {
				file1, file2, file_already_on_cloud
			});

			api.Setup(x => x.UploadAttachment(file1)).Verifiable();
			api.Setup(x => x.UploadAttachment(file2)).Verifiable();
			api.Setup(x => x.UploadAttachment(file_already_on_cloud)).Throws(new Exception("already uploaded"));

			db.Setup(x => x.UpdateFileOnCloud(file1)).Verifiable();
			db.Setup(x => x.UpdateFileOnCloud(file2)).Verifiable();

			task.Process(label);

			api.Verify();
			db.Verify();
		}

		[TestMethod]
		public void create_post_for_label_having_no_post_id()
		{
			var label = new Label { label_id = Guid.NewGuid(), seq = 500, share_proc_seq = 100 };

			api.Setup(x => x.CreatePost(label, null, It.IsAny<ICollection<FileAsset>>())).Verifiable();

			task.Process(label);

			db.Verify();
			api.Verify();
		}

		[TestMethod]
		public void update_post_for_label_having_post_id()
		{
			var label = new Label { label_id = Guid.NewGuid(), seq = 500, share_proc_seq = 100, share_post_id = "post_id" };

			api.Setup(x => x.UpdatePost(label, null, It.IsAny<ICollection<FileAsset>>())).Verifiable();

			task.Process(label);

			db.Verify();
			api.Verify();
		}

		[TestMethod]
		public void update_process_share_complete()
		{
			var label = new Label { label_id = Guid.NewGuid(), seq = 500, share_proc_seq = 100, share_post_id = "post_id" };

			db.Setup(x => x.UpdateShareComplete(label)).Verifiable();

			task.Process(label);

			db.Verify();
		}
	}
}
