using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Waveface.Model;

namespace Waveface.Client
{
	public class ContentTypeCountConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType,
	Object parameter, CultureInfo culture)
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

		public Object ConvertBack(Object value, Type targetType,
			Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
