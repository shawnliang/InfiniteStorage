using System;
using System.Globalization;
using System.Windows.Data;

namespace Waveface.Client
{
	public class MultiLineStringConvertor : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			var str = value as String;
			
			if (str == null)
				return null;

			return str.Replace("\\r\\n","\r\n");
		}

		public Object ConvertBack(Object value, Type targetType,
			Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
