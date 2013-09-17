#region

using System;
using Microsoft.Win32;

#endregion

namespace InfiniteStorage.Data
{
	public static class ProgramConfig
	{
		public static Uri WebBaseUri { get; private set; }
		public static Uri ApiBaseUri { get; private set; }
		public static Uri UserTrackUri { get; private set; }

		static ProgramConfig()
		{
			var webServer = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "WebServer", "https://devweb.waveface.com");
			WebBaseUri = new Uri(webServer);

			var apiServer = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ApiServer", "https://develop.waveface.com");
			ApiBaseUri = new Uri(apiServer);

			var userTrackUri = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "UserTrackUri", "https://dev.waveface.com/api/usertrack");
			UserTrackUri = new Uri(userTrackUri);
		}

		public static string FromWebBase(string relativePath)
		{
			var b = new UriBuilder(WebBaseUri);
			b.Path = relativePath;

			return b.ToString();
		}

		public static string FromApiBase(string relativePath)
		{
			var b = new UriBuilder(ApiBaseUri);
			b.Path = relativePath;

			var url = b.ToString();

			if (url.EndsWith("/"))
				return url.Substring(0, url.Length - 1);
			else
				return url;
		}
	}
}