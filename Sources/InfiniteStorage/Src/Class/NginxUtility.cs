using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InfiniteStorage
{
	class NginxUtility
	{
		const string TEMPLATE_FILE_NAME = "nginx.conf.template";

		private static string install_dir;
		private static string nginx_dir;
		private static NginxUtility instance = new NginxUtility();

		static NginxUtility()
		{
			install_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			nginx_dir = Path.Combine(install_dir, "nginx");
		}

		private NginxUtility()
		{
		}

		public static NginxUtility Instance
		{
			get { return instance; }
		}

		public void PrepareNginxConfig(string cfg_dir, ushort port, string origFileDir)
		{
			var install_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var nginx_dir = Path.Combine(install_dir, "nginx");
			var log_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny", "logs");
			var temp_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny", "kemp");

			File.Copy(Path.Combine(nginx_dir, @"conf\mime.types"), Path.Combine(cfg_dir, "mime.types"));

			using (var template = new StreamReader(Path.Combine(nginx_dir, @"conf\nginx.conf.template")))
			using (var target_cfg = new StreamWriter(Path.Combine(cfg_dir, "nginx.cfg")))
			{
				while (!template.EndOfStream)
				{
					var line = template.ReadLine();

					line = line.Replace("${listen}", port.ToString());
					line = line.Replace("${root}", origFileDir);
					line = line.Replace("${access_log}", Path.Combine(log_dir, "access.log"));
					line = line.Replace("${error_log}", Path.Combine(log_dir, "error.log"));
					line = line.Replace("${pid}", Path.Combine(log_dir, "pid.file"));

					line = line.Replace("${client_body_temp_path}", temp_dir);
					line = line.Replace("${proxy_temp_path}", temp_dir);
					line = line.Replace("${fastcgi_temp_path}", temp_dir);
					line = line.Replace("${uwsgi_temp_path}", temp_dir);
					line = line.Replace("${scgi_temp_path}", temp_dir);


					target_cfg.WriteLine(line);
				}
			}

			if (!Directory.Exists(log_dir))
				Directory.CreateDirectory(log_dir);

			if (!Directory.Exists(temp_dir))
				Directory.CreateDirectory(temp_dir);
		}

		public void Start(string cfg_dir)
		{

			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{

				Process[] processes;

				do
				{
					processes = Process.GetProcessesByName("nginx");
					if (processes != null)
					{
						Stop(cfg_dir);
						foreach (var p in processes)
						{
							try
							{
								p.Kill();
							}
							catch
							{

							}
						}
					}

					start(cfg_dir);
					reload(cfg_dir);

					processes = Process.GetProcessesByName("nginx");
				}
				while (processes == null || !processes.Any());
			});
		}

		private void start(string cfg_dir)
		{
			cmd(cfg_dir, "");
		}

		public void Stop(string cfg_dir)
		{
			cmd(cfg_dir, "-s stop");
		}

		private void reload(string cfg_dir)
		{
			cmd(cfg_dir, "-s reload");
		}

		private void cmd(string cfg_dir, string cmd)
		{
			var arg = string.Format("-c \"{0}\"", Path.Combine(cfg_dir, "nginx.cfg"));

			var nginx_exe = Path.Combine(nginx_dir, "nginx.exe");

			var p = new Process();

			p.StartInfo = new ProcessStartInfo
			{
				FileName = nginx_exe,
				Arguments = arg + " " + cmd,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false
			};
			p.Start();
		}
	}
}
