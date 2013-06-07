using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Wpf_testHTTP
{
    #region MultipartFormDataPostHelper

    public static class MultipartFormDataPostHelper
    {
        public static Encoding encoding = Encoding.UTF8;

        // Post the data as a multipart form
        // postParameters with a value of type byte[] will be passed in the form as a file, and value of type string will be
        // passed as a name/value pair.
        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters, string fileName, string mimeType)
        {
            string formDataBoundary = "--ABCDEFG";
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary, fileName, mimeType);

            return PostForm(postUrl, userAgent, contentType, formData);
        }


        public static HttpWebResponse PostWammerImage(string postUrl, string userAgent, Dictionary<string, object> postParameters, string fileName, string mimeType)
        {
            string formDataBoundary = "--ABCDEFG";
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary, fileName, mimeType);

            return PostForm(postUrl, userAgent, contentType, formData);
        }

        // Post a form
        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Add these, as we're doing a POST
            // request.KeepAlive = false;
            // request.Timeout = 10000;

            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();

            // We need to count how many bytes we're sending. 
            request.ContentLength = formData.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Push it out there
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            HttpWebResponse _resp = request.GetResponse() as HttpWebResponse;
            return request.GetResponse() as HttpWebResponse;
        }

        // Turn the key and value pairs into a multipart form.
        // See http://www.ietf.org/rfc/rfc2388.txt for issues about file uploads
        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary, string fileName, string mimeType)
        {
            Stream formDataStream = new MemoryStream();

            foreach (var param in postParameters)
            {
                if (param.Value is byte[])
                {
                    byte[] fileData = param.Value as byte[];

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string _header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n", boundary, param.Key, fileName.Equals("") ? param.Key : fileName, mimeType.Equals("") ? "application/octet-stream" : mimeType);

                    //formDataStream.Write(encoding.GetBytes(_header), 0, _header.Length);

                    byte[] _bytes = encoding.GetBytes(_header);
                    formDataStream.Write(_bytes, 0, _bytes.Length);

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileData, 0, fileData.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, param.Key, param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, postData.Length);
                }
            }

            // Add the end of the request
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }

    #endregion

    #region HttpHelp

    public class HttpHelp
    {
        public static bool EnableUnsafeHeaderParsing()
        {
            ////Get the assembly that contains the internal class
            //Assembly aNetAssembly = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));

            //if (aNetAssembly != null)
            //{
            //    //Use the assembly in order to get the internal type for the internal class
            //    Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");

            //    if (aSettingsType != null)
            //    {
            //        //Use the internal static property to get an instance of the internal settings class.
            //        //If the static instance isn't created allready the property will create it for us.
            //        object anInstance = aSettingsType.InvokeMember("Section",
            //          BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });

            //        if (anInstance != null)
            //        {
            //            //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
            //            FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
            //            if (aUseUnsafeHeaderParsing != null)
            //            {
            //                aUseUnsafeHeaderParsing.SetValue(anInstance, true);
            //                return true;
            //            }
            //        }
            //    }
            //}

            return false;
        }

        /// <summary>
        /// Function to download Image from website
        /// </summary>
        /// <param name="url">URL address to download image</param>
        /// <returns>Image</returns>
        public static Image DownloadImage(string url)
        {
            Image _tmpImage = null;

            try
            {
                // Open a connection
                HttpWebRequest _httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                _httpWebRequest.AllowWriteStreamBuffering = true;

                // You can also specify additional header values like the user agent or the referer: (Optional)
                _httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _httpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 20 seconds (Optional)
                _httpWebRequest.Timeout = 20000;

                // Request response:
                WebResponse _webResponse = _httpWebRequest.GetResponse();

                // Open data stream:
                Stream _webStream = _webResponse.GetResponseStream();

                // convert webstream to image
                _tmpImage = Image.FromStream(_webStream);

                // Cleanup
                _webResponse.Close();
                _webResponse.Close();
            }
            catch (Exception _e)
            {
                Console.WriteLine("Exception caught in process: {0}", _e);
                return null;
            }

            return _tmpImage;
        }

        /// <summary>
        /// Function to download HTML web page
        /// </summary>
        /// <param name="url">URL address to download web page</param>
        /// <returns>HTML contents as a string</returns>
        public static string DownloadHTMLPage(string url)
        {
            string _pageContent = null;

            try
            {
                // Open a connection
                HttpWebRequest _httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                // You can also specify additional header values like the user agent or the referer: (Optional)
                _httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _httpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 10 seconds (Optional)
                _httpWebRequest.Timeout = 10000;

                // Request response:
                WebResponse _webResponse = _httpWebRequest.GetResponse();

                // Open data stream:
                Stream _webStream = _webResponse.GetResponseStream();

                // Create reader object:
                StreamReader _streamReader = new StreamReader(_webStream);

                // Read the entire stream content:
                _pageContent = _streamReader.ReadToEnd();

                // Cleanup
                _streamReader.Close();
                _webStream.Close();
                _webResponse.Close();
            }
            catch (Exception _e)
            {
                Console.WriteLine("Exception caught in process: {0}", _e);
                return null;
            }

            return _pageContent;
        }

        /// <summary>
        /// Function to download a file from URL and save it to local drive
        /// </summary>
        /// <param name="url">URL address to download file</param>
        public static void DownloadFile(string url, string saveAs)
        {
            try
            {
                WebClient _webClient = new WebClient();

                // Downloads the resource with the specified URI to a local file.
                _webClient.DownloadFile(url, saveAs);
            }
            catch (Exception _e)
            {
                Console.WriteLine("Exception caught in process: {0}", _e);
            }
        }

        //
        //
        // 非同步
        //
        //// Occurs when an asynchronous file download operation completes.
        //private void _DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    // File download completed
        //    download_button.Enabled = Enabled;
        //    MessageBox.Show("Download completed");
        //}
        //
        //// Occurs when an asynchronous download operation successfully transfers some or all of the data.
        //private void _DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        //{
        //    // Update progress bar
        //    progressBar1.Value = e.ProgressPercentage;
        //}
        //
        //
        //// download button click event
        //private void download_button_Click(object sender, EventArgs e)
        //{
        //    // Disable download button to avoid clicking again while downloading the file
        //    download_button.Enabled = false;
        //
        //    // Downloads, to a local file, the resource with the specified URI. 
        //    // This method does not block the calling thread.
        //    System.Net.WebClient _WebClient = new System.Net.WebClient();
        //    _WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(_DownloadFileCompleted);
        //    _WebClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(_DownloadProgressChanged);
        //    _WebClient.DownloadFileAsync(new Uri("http://www.youdomain.com/sample-file.zip"), @"C:\sample-file.zip");
        //}

    }

    #endregion

    #region MyWebClient

    public class MyWebClient : WebClient
    {
        //time in milliseconds
        public int Timeout { get; set; }

        public MyWebClient()
        {
            Timeout = 60000;
        }

        public MyWebClient(int timeout)
        {
            Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var _result = base.GetWebRequest(address);
            _result.Timeout = this.Timeout;
            return _result;
        }
    }

    #endregion
}