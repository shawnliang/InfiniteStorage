using InfiniteStorage.Model;
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
				if (db.Object.Files.Any())
					seq = db.Object.Files.Max(x => x.seq);
			}
		}
	}
}
