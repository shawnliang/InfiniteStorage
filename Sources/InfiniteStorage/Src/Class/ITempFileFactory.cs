using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage
{
	public interface ITempFileFactory
	{
		/// <summary>
		/// Creates a temp file
		/// </summary>
		/// <returns>a temp file</returns>
		ITempFile CreateTempFile();
	}

	public interface ITempFile
	{
		/// <summary>
		/// Gets the path of the temp file
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Gets the number of bytes written to the temp file
		/// </summary>
		long BytesWritten { get; }

		/// <summary>
		/// Writes data to the end of this temp file
		/// </summary>
		/// <param name="data"></param>
		void Write(byte[] data);

		/// <summary>
		/// Ends writing this temp file. Closes the underlying file handle.
		/// </summary>
		void EndWrite();

		/// <summary>
		/// Drops this temp file and deletes it from the file system
		/// </summary>
		void Delete();
	}

	public interface IFileStorage
	{
		string MoveToStorage(string tempfile, FileContext file);
		void setDeviceName(string name);
	}
}
