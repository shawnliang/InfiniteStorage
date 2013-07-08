using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;

namespace InfiniteStorage.Share
{
	public interface IShareCloudAPI
	{
		void UploadAttachment(FileAsset file);

		void CreatePost(Label label, ICollection<ShareRecipient> recipients, ICollection<FileAsset> files);

		void UpdatePost(Label label, ICollection<ShareRecipient> recipients, ICollection<FileAsset> files);
	}
}
