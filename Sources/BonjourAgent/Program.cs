using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using System.Threading;

namespace BonjourAgent
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var options = new Options();

				if (!CommandLine.Parser.Default.ParseArguments(args, options))
					Environment.Exit(1);

				if (options.help)
				{
					Console.WriteLine(options.GetUsage());
				}

				using (var bonjour = new BonjourService(options.server_name))
				{
					bonjour.Register(options.backup_port, options.notify_port, options.rest_port, options.server_id, options.is_accepting, options.home_sharing);

					Console.Read();
				}

			}
			catch
			{

			}
		}
	}


	class Options
	{
		[Option("server-name", Required = true)]
		public string server_name { get; set; }

		[Option("server-id", Required=true)]
		public string server_id { get; set; }

		[Option("backup-port", Required = true)]
		public ushort backup_port { get; set; }

		[Option("notify-port", Required = true)]
		public ushort notify_port { get; set; }

		[Option("rest-port", Required = true)]
		public ushort rest_port { get; set; }

		[Option("is-accepting", DefaultValue=false)]
		public bool is_accepting { get; set; }

		[Option("home-sharing", DefaultValue = false)]
		public bool home_sharing { get; set; }

		[Option("help")]
		public bool help { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this,
			  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
