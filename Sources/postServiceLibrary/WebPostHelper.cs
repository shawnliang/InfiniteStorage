using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace postServiceLibrary
{
	class WebPostHelper
	{
		// private static Logger s_logger = LogManager.GetCurrentClassLogger();

		//回傳的網頁內容
		private String m_buff;

		//記錄Session及Cookie,如果登入時Session將一直存在
		// private static CookieContainer s_cookie;

		public WebPostHelper()
		{
			// s_cookie = new CookieContainer();
		}

		//取得網頁內容
		public string getContent()
		{
			return m_buff;
		}

		public string doPost(string sUrl, string data, string referer)
		{
			HttpWebRequest _urlConn = null;

			try
			{
				_urlConn = (HttpWebRequest)WebRequest.Create(sUrl);

				//連線最大等待時間
				//_urlConn.Timeout = 10000;
				_urlConn.Method = "POST";
				//_urlConn.ServicePoint.Expect100Continue = false;

				// ### because Google API don't accept these ---------------------------------------------------------
				//_urlConn.Headers.Set("Content-Encoding", "UTF-8");
				//_urlConn.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
				//_urlConn.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

				//設定referer
				if (referer != null)
				{
					_urlConn.Referer = referer;
				}

				_urlConn.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

				//自動重導
				// _urlConn.AllowAutoRedirect = true;

				if (data == null)
					data = "";

				//把要傳送的資料變成binary
				byte[] _bytes = Encoding.UTF8.GetBytes(data);
				_urlConn.ContentLength = _bytes.Length;

				//設定Cookie,Session
				//_urlConn.CookieContainer = ""; // s_cookie;

				//送出post資料
				if (data.Length > 0)
				{
					Stream _oStreamOut = _urlConn.GetRequestStream();
					_oStreamOut.Write(_bytes, 0, _bytes.Length);
					_oStreamOut.Close();
				}

				//取回回傳內容
				m_buff = (new StreamReader(_urlConn.GetResponse().GetResponseStream())).ReadToEnd();

				return m_buff;
			}
			catch (WebException _e)
			{
				//  NLogUtility.WebException(s_logger, _e, "doPost", false);
				if (_e.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse _res = (HttpWebResponse)_e.Response;

					using (var reader = new StreamReader(_res.GetResponseStream()))
					{
						var errJson = reader.ReadToEnd();
						var errObj = JsonConvert.DeserializeObject<dynamic>(errJson);

						throw new Exception(errObj.api_ret_message.ToString(), _e);
					}
				}
				else
					throw;
			}
			finally
			{
				try
				{
					if (_urlConn != null)
					{
						_urlConn.GetResponse().Close();
						_urlConn.GetRequestStream().Close();
					}
				}
				catch
				{
				}
			}
		}
	}
}
