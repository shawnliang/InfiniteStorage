namespace InfiniteStorage
{
	public interface IFileStorageLocationProvider
	{
		string PhotoLocation { get; }
		string VideoLocation { get; }
		string AudioLocation { get; }
	}

	internal class FileStorageLocationProvider : IFileStorageLocationProvider
	{
		public string PhotoLocation
		{
			get { return MyFileFolder.Photo; }
		}

		public string VideoLocation
		{
			get { return MyFileFolder.Video; }
		}

		public string AudioLocation
		{
			get { return MyFileFolder.Audio; }
		}
	}
}