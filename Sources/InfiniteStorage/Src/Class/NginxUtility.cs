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

		private Process nginxProcess;
		private bool stopping = false;

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


			var resFolder = Path.Combine(origFileDir, ".resource");
			if (!Directory.Exists(resFolder))
			{

				var dir = new DirectoryInfo(resFolder);
				dir.Create();
				dir.Attributes |= FileAttributes.Hidden;
			}

			copyFolder(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources"), resFolder);
		}

		private static void copyFolder(string from, string to)
		{
			var froms = Directory.GetFiles(from);
			foreach (var file in froms)
			{
				var file_name = Path.GetFileName(file);

				File.Copy(file, Path.Combine(to, file_name));
			}
		}

		public void Start(string cfg_dir)
		{
			stopping = false;
			var processes = Process.GetProcessesByName("nginx");
			if (processes != null)
			{
				cmd(cfg_dir, "-s stop");
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

			nginxProcess = start(cfg_dir, (s, e) =>
			{
				if (!stopping)
					Start(cfg_dir);
			});

			reload(cfg_dir);
		}

		private Process start(string cfg_dir, EventHandler onExit = null)
		{
			return cmd(cfg_dir, "", onExit);
		}

		public void Stop(string cfg_dir)
		{
			stopping = true;
			cmd(cfg_dir, "-s stop");
		}

		private void reload(string cfg_dir)
		{
			cmd(cfg_dir, "-s reload");
		}

		private Process cmd(string cfg_dir, string cmd, EventHandler onExit = null)
		{
			var arg = string.Format("-c \"{0}\"", Path.Combine(cfg_dir, "nginx.cfg"));

			var nginx_exe = Path.Combine(nginx_dir, "nginx.exe");

			var p = new Process();

			if (onExit != null)
			{
				p.EnableRaisingEvents = true;
				p.Exited += onExit;
			}

			p.StartInfo = new ProcessStartInfo
			{
				FileName = nginx_exe,
				Arguments = arg + " " + cmd,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false
			};
			p.Start();

			return p;
		}
	}
}
