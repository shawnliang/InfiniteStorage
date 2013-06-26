using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Drawing;
using System.Web;
using log4net;
using Newtonsoft;


namespace postServiceLibrary
{
	class newPostClass
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(newPostClass));

		public string APIKEY { get; set; }
		public string HostIp { get; set; }
		public string session_token { get; set; }
		public string group_id { get; set; }
		public string _object_id { get; set; }
		public string _responseMessage { get; set; }



		// last_update_time ???
		public void posts_update(string session_token, string group_id, string post_id, string attachment_id_array, string type, string event_type, string favorite, DateTime last_update_time)
		{
			log.Info("start Post_update");
			string _re = null;

			string _post_id = post_id;
			_post_id = System.Web.HttpUtility.UrlEncode(_post_id);
			session_token = System.Web.HttpUtility.UrlEncode(session_token);
			group_id = System.Web.HttpUtility.UrlEncode(group_id);
			attachment_id_array = System.Web.HttpUtility.UrlEncode(attachment_id_array);
			event_type = System.Web.HttpUtility.UrlEncode(event_type);
			favorite = System.Web.HttpUtility.UrlEncode(favorite);
			var lastUpdateTime = last_update_time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");


			string _url = "https://develop.waveface.com:443/v3" + "/pio_posts/update";
			DateTime _update = DateTime.UtcNow;
			string _dateStr = _update.ToString(@"yyyy-MM-ddTHH\:mm\:ssZ");

			string _parms = "post_id" + "=" + _post_id + "&" +
				"apikey" + "=" + APIKEY + "&" +
				"session_token" + "=" + session_token + "&" +
				"type" + "=" + type + "&" +
				"favorite" + "=" + favorite + "&" +
				"update_time" + "=" + _dateStr + "&" +
				"event_type" + "=" + event_type + "&" + 
				"last_update_time" + "=" + lastUpdateTime + "&";



			if (attachment_id_array != null)
				_parms += "attachment_id_array" + "=" + attachment_id_array + "&";

			_parms += "group_id" + "=" + group_id;


			WebPostHelper _webPos = new WebPostHelper();
			_webPos.doPost(_url, _parms, null);
		}

		#region new post

		public string posts_new(string session_token, string group_id, string content, string attachment_id_array,
							   string preview, string type, string coverAttach, string share_email_list, string event_type, string favorite, string post_id)
		{
			log.Info("start Post_new");
			string _re = null;

			post_id = System.Web.HttpUtility.UrlEncode(post_id);
			session_token = System.Web.HttpUtility.UrlEncode(session_token);
			group_id = System.Web.HttpUtility.UrlEncode(group_id);
			content = System.Web.HttpUtility.UrlEncode(content);
			attachment_id_array = System.Web.HttpUtility.UrlEncode(attachment_id_array);
			preview = System.Web.HttpUtility.UrlEncode(preview);
			share_email_list = System.Web.HttpUtility.UrlEncode(share_email_list);
			event_type = System.Web.HttpUtility.UrlEncode(event_type);
			favorite = System.Web.HttpUtility.UrlEncode(favorite);

			string _url = "https://develop.waveface.com:443/v3" + "/pio_posts/new";

			string _parms = "post_id" + "=" + post_id + "&" +
				"apikey" + "=" + APIKEY + "&" +
				"session_token" + "=" + session_token + "&" +
				"content" + "=" + content + "&" +
				"type" + "=" + type + "&" +
				"favorite" + "=" + favorite + "&" +
				"event_type" + "=" + event_type + "&";

			if (share_email_list != string.Empty)
				_parms += "shared_email_list" + "=" + share_email_list + "&";

			if (attachment_id_array != string.Empty)
				_parms += "attachment_id_array" + "=" + attachment_id_array + "&";

			if (preview != string.Empty)
				_parms += "preview" + "=" + preview + "&";

			if (type == "image")
			{
				if (coverAttach != string.Empty)
				{
					_parms += "cover_attach" + "=" + coverAttach + "&";
				}
			}

			_parms += "group_id" + "=" + group_id;


			WebPostHelper _webPos = new WebPostHelper();
			return _webPos.doPost(_url, _parms, null);
			
		}
		#endregion

		#region login

		public string auth_login(string user, string password)
		{
			string result = "";
			string email = user;
			user = System.Web.HttpUtility.UrlEncode(user);
			password = System.Web.HttpUtility.UrlEncode(password);

			Dictionary<string, object> _args = new Dictionary<string, object>();
			{
				_args.Add("apikey", APIKEY);
				_args.Add("email", email);
				_args.Add("password", password);
			};

			var postData = "apikey=" + APIKEY +
						   "&email=" + email +
						   "&password=" + password;


			string uri = "https://develop.waveface.com:443/v3/pio_auth/login"; // "https://develop.waveface.com:443/v3/pio_auth/login";

			var post = new WebPostHelper();
			post.doPost(uri, postData, "");

			var response = JsonConvert.DeserializeObject<MR_auth_login>(post.getContent());
			group_id = response.groups.First().group_id;
			session_token = response.session_token;

			return session_token;
		}
		#endregion
	}

	public class _Error
	{
		public string ProtocolError { get; set; }
		public string Description { get; set; }
	}

}