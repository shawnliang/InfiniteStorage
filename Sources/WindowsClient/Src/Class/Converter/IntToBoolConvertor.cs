using System;
using System.Globalization;
using System.Windows.Data;

namespace Waveface.Client
{
	public class IntToBoolConvertor : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			try
			{
				if (value is Int32)
				{
					var count = (Int32)value;

					return count != 0;
				}
				else
					return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public Object ConvertBack(Object value, Type targetType,
			Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
