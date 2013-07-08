using System.IO;

namespace InfiniteStorage
{
	class Log4netConfigure
	{
		public static void InitLog4net()
		{
			var configFile = getLog4netConfigFilePath();

			var configFileInfo = new FileInfo(configFile);

			if (!configFileInfo.Exists || configFileInfo.Length == 0)
#if DEBUG
				createDefaulConfigFile(configFile, DebugLevel.DEBUG);
#else
				createDefaulConfigFile(configFile, DebugLevel.WARN);
#endif


			log4net.Config.XmlConfigurator.ConfigureAndWatch(configFileInfo);
		}

		private static string getLog4netConfigFilePath()
		{
			var productFolder = MyFileFolder.AppData;

			if (!Directory.Exists(productFolder))
				Directory.CreateDirectory(productFolder);

			return Path.Combine(productFolder, "log4net.config");
		}

		private static void createDefaulConfigFile(string configFile, DebugLevel level)
		{
			using (var writer = new StreamWriter(configFile))
			{
				writer.WriteLine(@"<?xml version='1.0'?>

<log4net>
	<appender name='RollingFile' type='log4net.Appender.RollingFileAppender'>
		<file value='${APPDATA}\Bunny\server.log' />
		<appendToFile value='true' />
		<maximumFileSize value='10MB' />
		<maxSizeRollBackups value='10' />

		<layout type='log4net.Layout.PatternLayout'>
			<conversionPattern value='%date [%thread] %-5level %logger - %message%newline' />
		</layout>
	</appender>

	<root>
		<level value='" + level.ToString() + @"' />
		<appender-ref ref='RollingFile' />
	</root>
</log4net>");
			}
		}


		public static void SetLevel(DebugLevel level)
		{
			var log4netCfgFile = getLog4netConfigFilePath();
			createDefaulConfigFile(log4netCfgFile, level);
		}
	}
}
