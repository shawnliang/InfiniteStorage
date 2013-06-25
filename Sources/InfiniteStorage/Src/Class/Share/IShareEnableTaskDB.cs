using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;

namespace InfiniteStorage.Share
{
	public interface IShareEnableTaskDB
	{
		ICollection<FileAsset> QueryLabelFiles(Label label);

		void UpdateFileOnCloud(FileAsset file);

		ICollection<ShareRecipient> QueryRecipients(Label label);

		void UpdateRecipientOnCloud(ShareRecipient recipient);

		void UpdateShareComplete(Label label);
	}
}
