using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage
{
	public class Normalizer
	{
		public static void NormalizeToInt(long val1, long val2, out int norVal1, out int norVal2)
		{
			int shiftBits = 0;
			long val = (val1 > val2) ? val1 : val2;

			while (val > int.MaxValue)
			{
				val = (val >> 1);
				shiftBits += 1;
			}

			norVal1 = (int)(val1 >> shiftBits);
			norVal2 = (int)(val2 >> shiftBits);
		}
	}
}
