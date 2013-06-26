using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace InfiniteStorage.Common
{
	static class FFmpegHelper
	{
		static string program_path;

		static FFmpegHelper()
		{
			program_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffmpeg.exe");
		}


		public static void MakeVideoThumbnail(string video_path, string thumb_path)
		{
			var args = string.Format("-i \"{0}\" -an -vcodec libx264 -crf 23 \"{1}\"", video_path, thumb_path);

			makeVideoThumbnail(thumb_path, args);
		}

		public static void MakeVideoThumbnail720(string video_path, string thumb_path)
		{
			var args = string.Format("-i \"{0}\" -an -vcodec libx264 -crf 23 -vf \"scale=720:trunc(ow/a/2)*2\" \"{1}\"", video_path, thumb_path);

			makeVideoThumbnail(thumb_path, args);
		}

		private static void makeVideoThumbnail(string thumb_path, string args)
		{
			if (File.Exists(thumb_path))
				File.Delete(thumb_path);

			using (var proc = new Process())
			{
				proc.StartInfo = new ProcessStartInfo
				{
					FileName = program_path,
					Arguments = args,
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden
				};
				proc.Start();
				proc.PriorityClass = ProcessPriorityClass.BelowNormal;

				proc.WaitForExit();
			}
		}
	}
}
