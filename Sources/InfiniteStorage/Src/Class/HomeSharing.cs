using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace InfiniteStorage
{
	static class HomeSharing
	{
		public static bool Enabled
		{
			get
			{
				return Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "HomeSharing", "true").Equals("true");
			}

			set
			{
				Registry.SetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "HomeSharing", value ? "true" : "false");
			}
		}
	}
}
