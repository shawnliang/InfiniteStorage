#region

using Microsoft.Win32;

#endregion

namespace InfiniteStorage
{
	public static class HomeSharing
	{
		public static bool Enabled
		{
			get { return Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "HomeSharing", "true").Equals("true"); }

			set { Registry.SetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "HomeSharing", value ? "true" : "false"); }
		}
	}
}