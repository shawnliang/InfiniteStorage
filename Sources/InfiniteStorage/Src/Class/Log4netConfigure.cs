using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InfiniteStorage.Properties;

namespace InfiniteStorage.Src.Class
{
	class Log4netConfigure
	{
		public static void InitLog4net()
		{
			var appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var productFolder = Path.Combine(appdataFolder, Resources.ProductName);
			var configFile = Path.Combine(productFolder, "log4net.config");

			var configFileInfo = new FileInfo(configFile);

			if (!configFileInfo.Exists)
				createDefaulConfigFile(configFile);

			log4net.Config.XmlConfigurator.ConfigureAndWatch(configFileInfo);
		}

		private static void createDefaulConfigFile(string configFile)
		{
			using (var writer = new StreamWriter(configFile))
			{
				writer.WriteLine(@"
<?xml version='1.0'?>

  <log4net>

	<appender name='RollingFile' type='log4net.Appender.RollingFileAppender'>
	  <file value='${APPDATA}\InfiniteStorage\server.log' />
	  <appendToFile value='true' />
	  <maximumFileSize value='10MB' />
	  <maxSizeRollBackups value='10' />

	  <layout type='log4net.Layout.PatternLayout'>
		<conversionPattern value='%date [%thread] %-5level %logger - %message%newline' />
	  </layout>
	</appender>

	<root>
	  <level value='INFO' />
	  <appender-ref ref='RollingFile' />
	</root>
  </log4net>");
			}
		}
	}
}
