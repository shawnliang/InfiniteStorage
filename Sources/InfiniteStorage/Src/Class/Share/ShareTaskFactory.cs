
namespace InfiniteStorage.Share
{
	class ShareTaskFactory : IShareTaskFactory
	{
		public IShareTask CreateEnableTask()
		{
			return new ShareEnableTask();
		}

		public IShareTask CreateDisableTask()
		{
			return new ShareDisableTask();
		}
	}
}
