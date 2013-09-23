#region

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using InfiniteStorage.Properties;
using log4net;

#endregion

namespace InfiniteStorage
{
	internal class NginxUtility
	{
		//private const string TEMPLATE_FILE_NAME = "nginx.conf.template";

		private static string install_dir;
		private static string nginx_dir;
		private static NginxUtility instance = new NginxUtility();

		private Process nginxProcess;
		private bool stopping;

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

		public void PrepareNginxConfig(ushort port, string origFileDir)
		{
			var _installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var _nginxDir = Path.Combine(_installDir, "nginx");
			var _logDir = Path.Combine(MyFileFolder.AppData, "logs");
			var _tempDir = Path.Combine(MyFileFolder.AppData, "kemp");

			File.Copy(Path.Combine(_nginxDir, @"conf\mime.types"), Path.Combine(MyFileFolder.AppData, "mime.types"), true);

			if (Settings.Default.HomeSharingPasswordRequired)
			{
				using (var pwdFile = new StreamWriter(Path.Combine(MyFileFolder.AppData, "nginx.pwd")))
				{
					pwdFile.WriteLine("user:" + Settings.Default.HomeSharingPassword);
				}
			}

			var escaped_log_dir = _logDir.Replace("\'", @"\'");
			var escaped_temp_dir = _tempDir.Replace("\'", @"\'");
			var escaped_orig_file_dir = origFileDir.Replace("\'", @"\'");

			using (var template = new StreamReader(Path.Combine(_nginxDir, @"conf\nginx.conf.template")))
			using (var target_cfg = new StreamWriter(Path.Combine(MyFileFolder.AppData, "nginx.cfg")))
			{
				while (!template.EndOfStream)
				{
					var line = template.ReadLine();

					line = line.Replace("${listen}", port.ToString());
					line = line.Replace("${root}", escaped_orig_file_dir);
					line = line.Replace("${access_log}", Path.Combine(escaped_log_dir, "access.log"));
					line = line.Replace("${error_log}", Path.Combine(escaped_log_dir, "error.log"));
					line = line.Replace("${pid}", Path.Combine(escaped_log_dir, "pid.file"));

					line = line.Replace("${client_body_temp_path}", escaped_temp_dir);
					line = line.Replace("${proxy_temp_path}", escaped_temp_dir);
					line = line.Replace("${fastcgi_temp_path}", escaped_temp_dir);
					line = line.Replace("${uwsgi_temp_path}", escaped_temp_dir);
					line = line.Replace("${scgi_temp_path}", escaped_temp_dir);

					if (Settings.Default.HomeSharingPasswordRequired)
					{
						line = line.Replace("#auth_basic123", "auth_basic                \"Restricted\";");
						line = line.Replace("#auth_basic_user_file", "auth_basic_user_file      nginx.pwd;");
					}

					target_cfg.WriteLine(line);
				}
			}

			if (!Directory.Exists(_logDir))
				Directory.CreateDirectory(_logDir);

			if (!Directory.Exists(_tempDir))
				Directory.CreateDirectory(_tempDir);

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

				File.Copy(file, Path.Combine(to, file_name), true);
			}
		}

		public void Start()
		{
			var cfg_dir = MyFileFolder.AppData;

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
						                              Start();
				                              });

			reload(cfg_dir);
		}

		private Process start(string cfg_dir, EventHandler onExit = null)
		{
			return cmd(cfg_dir, "", onExit);
		}

		public void Stop()
		{
			stopping = true;
			cmd(MyFileFolder.AppData, "-s stop");
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

			LogManager.GetLogger("nginx").Debug(nginx_exe + " " + arg + " " + cmd);
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