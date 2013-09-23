#region

using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using InfiniteStorage.Data;

#endregion

namespace Waveface.ClientFramework
{
	public class UserTrackApi
	{
		private WebClient webAgent = new WebClient();

		public void CallAync(string os, string version, string stage, int rating, string comment, string action)
		{
			var param = new Dictionary<string, object>
				            {
					            {"os", os},
					            {"version", version},
					            {"stage", stage},
					            {"rating", rating},
					            {"comment", comment},
					            {"action", action}
				            };

			var buff = new StringBuilder();

			foreach (var par in param)
			{
				buff.Append(par.Key).Append("=").Append(HttpUtility.UrlEncode(par.Value.ToString())).Append("&");
			}

			var formData = buff.ToString().Substring(0, buff.Length - 1);
			webAgent.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
			webAgent.UploadDataCompleted += uploadCompleted;
			webAgent.UploadDataAsync(ProgramConfig.UserTrackUri, "POST", Encoding.UTF8.GetBytes(formData));
		}

		private void uploadCompleted(object sender, UploadDataCompletedEventArgs args)
		{
		}
	}
}