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
		public void posts_update(string session_token, string group_id, string post_id, string attachment_id_array, string last_update_time, string type, string event_type, string favorite)
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


			string _url = "https://develop.waveface.com:443/v3" + "/pio_posts/update";
			string _date = last_update_time;
			DateTime _update = DateTime.Now;
			string _dateStr = _update.ToString(@"yyyy-MM-ddTHH\:mm\:ssZ");

			string _parms = "post_id" + "=" + _post_id + "&" +
				"apikey" + "=" + APIKEY + "&" +
				"session_token" + "=" + session_token + "&" +
				"type" + "=" + type + "&" +
				"favorite" + "=" + favorite + "&" +
				"update_time" + "=" + _dateStr + "&" +
				"event_type" + "=" + event_type + "&" +
			   "last_update_time" + "=" + _date + "&";



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

		private string genGuid()
		{
			Guid g;
			// Create and display the value of two GUIDs.
			g = Guid.NewGuid();
			return g.ToString();
		}
		#endregion

		#region login
		// public string email { get; set; }
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

		#region attachment upload

		public string callUploadAttachment(string session_token, string filename)
		{
			string result = "";
			string title = "";
			string description = "";
			string type = "image";
			string image_data = "medium";
			string object_id = genGuid();
			string post_id = "";
			try
			{
				int i0 = filename.IndexOf(".mp4");
				if (i0 > 0)
				{
					type = "video";
				}
				MR_attachments_upload _attachment = attachments_upload(null, session_token, group_id, filename, title, description, type, image_data, object_id, post_id);
				result = _attachment.object_id;
			}
			catch (Exception err)
			{
				log.Error("upload error: " + err.Message);
				Console.WriteLine("Error: " + err.Message);
			}
			return result;
		}

		public MR_attachments_upload attachments_upload(byte[] file_data, string session_token, string group_id, string fileName,
														string title, string description, string type, string image_meta,
														string object_id, string post_id)
		{
			MR_attachments_upload _ret = null;

			try
			{
				string _url = "https://develop.waveface.com:443/v3" + "/pio_attachments/upload";

				string _mimeType = FileUtility.GetMimeType(new FileInfo(fileName));
				byte[] _data;
				if (file_data != null)
				{
					_data = file_data;
				}
				else
				{
					_data = FileUtility.ConvertFileToByteArray(fileName);
				}

				Dictionary<string, object> _dic = new Dictionary<string, object>();
				_dic.Add("apikey", APIKEY);
				_dic.Add("session_token", session_token);
				_dic.Add("group_id", group_id);
				_dic.Add("file_name", fileName);
				_dic.Add("title", title);
				//
				DateTime fileCreatedDate = File.GetCreationTime(fileName);
				string _date = fileCreatedDate.ToString(@"yyyy-MM-ddTHH\:mm\:ssZ");
				_dic.Add("file_create_time", _date);

				//if (description == string.Empty)
				//   description = title;

				_dic.Add("description", description);
				_dic.Add("type", type);

				if (type == "image")
					_dic.Add("image_meta", image_meta);

				if (object_id != string.Empty)
					_dic.Add("object_id", object_id);

				if (post_id != string.Empty)
					_dic.Add("post_id", post_id);

				_dic.Add("file", _data);

				string _userAgent = "Windows";

				string _fileName = new FileInfo(fileName).Name;

				HttpWebResponse _webResponse = MultipartFormDataPostHelper.MultipartFormDataPost(_url, _userAgent, _dic,
																								 _fileName, _mimeType);


				// Process response
				StreamReader _responseReader = new StreamReader(_webResponse.GetResponseStream());
				string _r = _responseReader.ReadToEnd();
				_webResponse.Close();

				_ret = JsonConvert.DeserializeObject<MR_attachments_upload>(_r);
			}
			catch (WebException _e)
			{
				//NLogUtility.WebException(s_logger, _e, "attachments_upload", true);

				if (_e.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse res = (HttpWebResponse)_e.Response;
					log.Error("upload attachments error response: " + res);
				}

			}
			catch (Exception _e)
			{

			}

			return _ret;
		}

		#endregion
	}

	public class _Error
	{
		public string ProtocolError { get; set; }
		public string Description { get; set; }
	}

}