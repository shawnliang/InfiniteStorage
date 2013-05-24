#region

using System;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

#endregion

namespace Waveface
{
    public class MyImageItem
    {
        public string ImageFilePath { get; set; }

        public BitmapImage RemoteImage
        {
            get
            {
                var _image = new BitmapImage();
                int _bytesToRead = 10240;

                WebRequest _request = WebRequest.Create(new Uri(ImageFilePath, UriKind.Absolute));
                _request.Timeout = -1;
                WebResponse _response = _request.GetResponse();
                Stream _responseStream = _response.GetResponseStream();
                BinaryReader _reader = new BinaryReader(_responseStream);
                MemoryStream _ms = new MemoryStream();

                byte[] _bytebuffer = new byte[_bytesToRead];
                int _bytesRead = _reader.Read(_bytebuffer, 0, _bytesToRead);

                while (_bytesRead > 0)
                {
                    _ms.Write(_bytebuffer, 0, _bytesRead);
                    _bytesRead = _reader.Read(_bytebuffer, 0, _bytesToRead);
                }

                _image.BeginInit();
                _ms.Seek(0, SeekOrigin.Begin);

                _image.StreamSource = _ms;
                _image.EndInit();

                return _image;
            }
        }
    }
}