#region

using System.IO;

#endregion

namespace InfiniteStorage
{
	public interface IFileMove
	{
		string Move(string src, string dest);
	}

	public class FileMover : IFileMove
	{
		public string Move(string src, string dest)
		{
			int num = 1;

			var oldDest = dest;

			while (true)
			{
				try
				{
					File.Move(src, dest);
					break;
				}
				catch (IOException e)
				{
					if (File.Exists(dest))
					{
						dest = Path.Combine(Path.GetDirectoryName(oldDest), Path.GetFileNameWithoutExtension(oldDest) + "." + num + Path.GetExtension(oldDest));
						num += 1;
					}
					else
						throw new IOException("Unable to move file to " + dest, e);
				}
			}

			return dest;
		}
	}
}