#region

using System;

#endregion

internal static class PathUtil
{
	public static string MakeRelative(string path, string base_path)
	{
		if (!path.StartsWith(base_path))
			throw new InvalidOperationException(base_path + " is not parent of " + path);

		var rel = path.Substring(base_path.Length);

		if (rel.StartsWith(@"\"))
			rel = rel.Substring(1);

		return rel;
	}
}