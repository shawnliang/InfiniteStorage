using InfiniteStorage.Model;
using System;
using System.Linq;
using System.Threading;

namespace InfiniteStorage
{
	static class SeqNum
	{
		private static long seq = 0;

		public static long GetNextSeq()
		{
			return Interlocked.Increment(ref seq);
		}

		public static void InitFromDB()
		{
			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.Files
						orderby f.seq descending
						select f.seq;


				var q2 = from f in db.Object.PendingFiles
						 orderby f.seq descending
						 select f.seq;

				var q3 = from f in db.Object.Labels
						 orderby f.seq descending
						 select f.seq;

				var max1 = q.Any() ? q.Max() : 0;
				var max2 = q2.Any() ? q2.Max() : 0;
				var max3 = q3.Any() ? q3.Max() : 0;

				seq = Math.Max(Math.Max(max1, max2), max3);
			}
		}
	}
}
