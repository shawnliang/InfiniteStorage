
namespace InfiniteStorage.Share
{
	public interface IShareTaskFactory
	{
		IShareTask CreateEnableTask();
		IShareTask CreateDisableTask();
	}

	public interface IShareTask
	{
		void Process(InfiniteStorage.Model.Label label);
	}
}
