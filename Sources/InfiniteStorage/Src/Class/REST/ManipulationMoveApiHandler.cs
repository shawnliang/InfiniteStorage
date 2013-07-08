using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace InfiniteStorage.REST
{
	class ManipulationMoveApiHandler : ManipulationApiHandlerBase
	{
		FileMover fileMover = new FileMover();

		public override void HandleRequest()
		{
			CheckParameter("targetPath");

			var file_ids = GetIds();
			var folders = GetPaths();
			var full_target_path = Parameters["targetPath"];


			var result = Manipulation.Manipulation.Move(file_ids, full_target_path);

			respondSuccess(new {
				api_ret_code = 0,
				api_ret_message = "success",
				status = 200,

				moved_files = result.moved_files,
				not_moved_files = result.not_moved_files
			});
		}
	}
}
