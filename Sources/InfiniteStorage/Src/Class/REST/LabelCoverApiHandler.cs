using System;
using System.Linq;
using System.Net;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class LabelCoverApiHandler : HttpHandler
	{

		public override void HandleRequest()
		{
			var args = Request.Url.AbsolutePath.Split(new string[] { @"/" }, StringSplitOptions.RemoveEmptyEntries);

			if (args.Length != 2)
				throw new FormatException("request url is not expected: " + Request.Url.AbsolutePath);

			var label_id = new Guid(args[1]);

			var util = new Notify.NotifySenderUtil();
			var files = util.QueryLabeledFiles(label_id);

			if (files.Any())
				Response.RedirectLocation = new UriBuilder(Request.Url.Scheme, Request.Url.Host, Request.Url.Port, "/image/" + files.First().id.ToString() + "/medium").ToString();
			else
				Response.RedirectLocation = new UriBuilder(Request.Url.Scheme, Request.Url.Host, 12888, "/.resource/Empty.png").ToString();

			Response.StatusCode = (int)HttpStatusCode.Found;
			Response.Close();
		}
	}
}
