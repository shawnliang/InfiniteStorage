using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	abstract class ManipulationApiHandlerBase : HttpHandler
	{
		protected List<Guid> GetIds()
		{
			return parseToList(Parameters["ids"]).Select(x => new Guid(x)).ToList();
		}

		protected List<string> GetPaths()
		{
			return parseToList(Parameters["paths"]).Select(x => PathUtil.MakeRelative(x, MyFileFolder.Photo)).ToList();
		}

		protected string[] parseToList(string para)
		{
			if (string.IsNullOrEmpty(para))
				return new string[] { };

			return para.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
