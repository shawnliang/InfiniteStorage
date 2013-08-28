using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Waveface.Model
{
	public static class ObservableCollectionExtension
	{
		public static void Sort<T>(this ObservableCollection<T> obj, Comparison<T> comparer)
		{
			var index = 1;
			while (index < obj.Count)
			{
				var i = index;
				var j = i - 1;
				while (j >= 0)
				{

					if (comparer(obj[i], obj[j]) < 0)
					{
						obj.Move(i, j);
						i = j;
						j--;
					}
					else
						break;
				}
				
				index++;
			}
		}
	}
}
