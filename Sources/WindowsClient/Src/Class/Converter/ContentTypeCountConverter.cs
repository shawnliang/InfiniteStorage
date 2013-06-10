using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Waveface.Model;
using System.Linq;

namespace Waveface.Client
{
	public class ContentTypeCountConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
	object parameter, CultureInfo culture)
		{
			try
			{
				var contentEntities = value as IEnumerable<IContentEntity>;

				if (value == null)
					return null;

				var count = contentEntities.Count(item =>
				{
					var content = item as IContent;
					if (content == null)
						return false;

					return content.Type == (ContentType)parameter;
				});

				return count;
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
