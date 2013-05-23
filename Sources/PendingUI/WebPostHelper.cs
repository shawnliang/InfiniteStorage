using System;
using System.IO;
using System.Net;
using System.Text;

namespace Waveface
{
    class WebPostHelper
    {
        //回傳的網頁內容
        private String m_buff;

        //記錄Session及Cookie,如果登入時Session將一直存在
        private static CookieContainer s_cookie;

        public WebPostHelper()
        {
            s_cookie = new CookieContainer();
        }

        //取得網頁內容
        public string getContent()
        {
            return m_buff;
        }

        public bool doPost(string sUrl, string data, string referer)
        {
            bool _dosuccess = false;
            HttpWebRequest _urlConn = null;

            try
            {
                _urlConn = (HttpWebRequest)WebRequest.Create(sUrl);

                //連線最大等待時間
                //_urlConn.Timeout = 10000;
                _urlConn.Method = "POST";
                //_urlConn.ServicePoint.Expect100Continue = false;

                _urlConn.Headers.Set("Content-Encoding", "UTF-8");
                _urlConn.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                _urlConn.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                //設定referer
                if (referer != null)
                {
                    _urlConn.Referer = referer;
                }

                _urlConn.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

                //自動從導
                // _urlConn.AllowAutoRedirect = true;

                if (data == null)
                    data = "";

                //把要傳送的資料變成binary
                byte[] _bytes = Encoding.UTF8.GetBytes(data);
                _urlConn.ContentLength = _bytes.Length;

                //設定Cookie,Session
                _urlConn.CookieContainer = s_cookie;

                //送出post資料
                if (data.Length > 0)
                {
                    Stream _oStreamOut = _urlConn.GetRequestStream();
                    _oStreamOut.Write(_bytes, 0, _bytes.Length);
                    _oStreamOut.Close();
                }

                //取回回傳內容
                m_buff = (new StreamReader(_urlConn.GetResponse().GetResponseStream())).ReadToEnd();

                _dosuccess = true;
            }
            catch (WebException _e)
            {
            }
            catch (Exception _e)
            {
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

            return _dosuccess;
        }
    }
}