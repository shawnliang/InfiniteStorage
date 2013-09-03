using InfiniteStorage.Model;
using System.Collections.Generic;

namespace InfiniteStorage.Share
{
	public interface IShareCloudAPI
	{
		void UploadAttachment(FileAsset file);

		void CreatePost(Label label, ICollection<ShareRecipient> recipients, ICollection<FileAsset> files);

		void UpdatePost(Label label, ICollection<ShareRecipient> recipients, ICollection<FileAsset> files);
	}
}
