#region

using System;
using System.Linq;
using System.Threading;
using InfiniteStorage.Model;

#endregion

namespace InfiniteStorage
{
	internal static class SeqNum
	{
		private static long seq;

		public static long GetNextSeq()
		{
			return Interlocked.Increment(ref seq);
		}

		public static void InitFromDB()
		{
			using (var db = new MyDbContext())
			{
				var q1 = from f in db.Object.Files
				        orderby f.seq descending
				        select f.seq;

				var q2 = from f in db.Object.Labels
				         orderby f.seq descending
				         select f.seq;

				var max1 = q1.Any() ? q1.Max() : 0;
				var max2 = q2.Any() ? q2.Max() : 0;

				seq = Math.Max(max1, max2);
			} 
		}
	}
}