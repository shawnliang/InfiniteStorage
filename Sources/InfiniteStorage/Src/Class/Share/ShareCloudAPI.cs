using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Share
{
	class ShareCloudAPI : IShareCloudAPI
	{
		public void UploadAttachment(Model.FileAsset file)
		{
			throw new NotImplementedException();
		}

		public void CreatePost(Model.Label label, ICollection<Model.ShareRecipient> recipients, ICollection<Model.FileAsset> files)
		{
			throw new NotImplementedException();
		}

		public void UpdatePost(Model.Label label, ICollection<Model.ShareRecipient> recipients, ICollection<Model.FileAsset> files)
		{
			throw new NotImplementedException();
		}
	}
}
