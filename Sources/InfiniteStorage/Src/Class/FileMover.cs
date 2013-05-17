using System.IO;

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
						dest = Path.Combine(Path.GetDirectoryName(dest), Path.GetFileNameWithoutExtension(dest) + "." + num + Path.GetExtension(dest));
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
