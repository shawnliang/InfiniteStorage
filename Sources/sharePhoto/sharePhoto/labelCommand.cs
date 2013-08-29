#region

using System.Web;

#endregion

namespace Wpf_testHTTP
{
	internal class labelCommand
	{
		#region reset command

		public bool debug = true;
		public static string _HostIP = "http://localhost:14005";
		public string current_device { get; set; }
		private static string invite_cmd = _HostIP + "/label/invite"; //?label_id=";            //...&recipients=...

		/*
		private string getAllLabel_cmd = _HostIP + "/label/list_all";
		private string getAll_cmd0 = _HostIP + "/pending/get?device_id="; //
		private string getAll_cmd1 = "&seq=0&limit=500";
		private string getNextFiles_cmd = _HostIP + "/pending/get?seq="; //10234&limit=500
		private string tagLabel_cmd = _HostIP + "/label/tag?file_id="; // + file_guid + "&label_id=" + label_id;"; 
		private string clearLabel_cmd = _HostIP + "label/clear?" + "label_id="; // + label_id;
		*/

		#endregion

		public static string inviteShared(string label_id, string name, string message, string recipients_json)
		{
			string result = null;

			if (string.IsNullOrEmpty(label_id))
			{
				return null;
			}

			string _url = invite_cmd;

			string _label_id = HttpUtility.UrlEncode(label_id);
			string _name = HttpUtility.UrlEncode(name);
			string _parms = "msg" + "=" + message + "&" +
			                "label_id" + "=" + _label_id + "&" +
			                "recipients" + "=" + recipients_json + "&" +
			                "sender" + "=" + _name;

			WebPostHelper _webPos = new WebPostHelper();
			bool _isOK = _webPos.doPost(_url, _parms, null);

			if (_isOK)
			{
				string _r = _webPos.getContent();
				// var results = JsonConvert.DeserializeObject<dynamic>(_r);             
				result = _r;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}