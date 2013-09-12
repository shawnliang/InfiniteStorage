using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Waveface.Client
{
	public class DoubleToIntConvertor : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			try
			{
				if (value is Double)
				{
					return new GridLength((Double)value);
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
