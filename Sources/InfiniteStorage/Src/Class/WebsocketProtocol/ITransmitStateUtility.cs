using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface ITransmitStateUtility
	{
		void SaveFileRecord(FileAsset file);
		bool HasDuplicateFile(FileContext file, string device_id);
	}
}
