using System;
using InfiniteStorage.Model;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface ITransmitStateUtility
	{
		void SaveFileRecord(FileAsset file, ProtocolContext ctx);
		bool HasDuplicateFile(FileContext file, string device_id);
		long GetNextSeq();

		Guid? QueryFileId(string device_id, string file_path);
	}

	public class SavedPath
	{
		public string device_folder { get; set; }
		public string relative_file_path { get; set; }
	}
}
