using InfiniteStorage.Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class PendingSortApiHandler : HttpHandler
	{

		public override void HandleRequest()
		{
			CheckParameter("how");

			var how = JsonConvert.DeserializeObject<PendingSortData>(Parameters["how"]);

			string folder_name = getDeviceFolder(how);

			var pendingToResource = new PendingToResource(
				new PendingToResourceUtil(MyDbContext.ConnectionString, folder_name, MyFileFolder.Photo));

			foreach (var evt in how.events)
			{
				pendingToResource.Do(evt);
			}

			respondSuccess();
		}

		private static string getDeviceFolder(PendingSortData how)
		{
			string folder_name;
			using (var db = new MyDbContext())
			{
				folder_name = (from d in db.Object.Devices
							   where d.device_id == how.device_id
							   select d.folder_name).FirstOrDefault();
			}

			if (string.IsNullOrEmpty(folder_name))
				throw new Exception("Unknown device_id:" + how.device_id);

			return folder_name;
		}
	}
}
