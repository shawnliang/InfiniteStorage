using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class DateTimeExtension
{
	public static DateTime TrimToDay(this DateTime time)
	{
		return new DateTime(time.Year, time.Month, time.Day, 0, 0, 0, time.Kind);
	}
}
