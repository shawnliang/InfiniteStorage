using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace Waveface.Client
{
	class CmdLineOptions
	{
		[Option("select-device", Required = false)]
		public string select_device { get; set; }
	}
}
