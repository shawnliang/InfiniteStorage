using System;
using System.Globalization;
using System.Windows.Data;

namespace Waveface.Client
{
	public class IntToBoolConvertor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				if (value is int)
				{
					var count = (int)value;

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

		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
