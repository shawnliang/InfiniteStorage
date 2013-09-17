#region

using System;
using CommandLine;
using CommandLine.Text;

#endregion

namespace BonjourAgent
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				var options = new Options();

				if (!Parser.Default.ParseArguments(args, options))
					Environment.Exit(1);

				if (options.help)
				{
					Console.WriteLine(options.GetUsage());
				}

				using (var bonjour = new BonjourService(options.server_name))
				{
					bonjour.Register(options.backup_port, options.notify_port, options.rest_port, options.server_id, options.is_accepting, options.home_sharing, options.passcode);

					Console.Read();
				}
			}
			catch
			{
			}
		}
	}


	internal class Options
	{
		[Option("server-name", Required = true)]
		public string server_name { get; set; }

		[Option("server-id", Required = true)]
		public string server_id { get; set; }

		[Option("backup-port", Required = true)]
		public ushort backup_port { get; set; }

		[Option("notify-port", Required = true)]
		public ushort notify_port { get; set; }

		[Option("rest-port", Required = true)]
		public ushort rest_port { get; set; }

		[Option("passcode", Required = true)]
		public string passcode { get; set; }

		[Option("is-accepting", DefaultValue = false)]
		public bool is_accepting { get; set; }

		[Option("home-sharing", DefaultValue = false)]
		public bool home_sharing { get; set; }

		[Option("help")]
		public bool help { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}