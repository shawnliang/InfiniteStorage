using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage.WebsocketProtocol;

namespace UnitTest
{
	[TestClass]
	public class testDirOrganizers
	{
		[TestMethod]
		public void testYYYY()
		{
			var org = new DirOrganizerByYYYY();

			var path = org.GetDir(new FileContext { datetime = new DateTime(2013, 5, 12, 12, 21, 31, DateTimeKind.Local) });

			Assert.AreEqual("2013", path);
		}

		[TestMethod]
		public void testYYYYMM()
		{
			var org = new DirOrganizerByYYYYMM();

			var path = org.GetDir(new FileContext { datetime = new DateTime(2013, 5, 12, 12, 21, 31, DateTimeKind.Local) });

			Assert.AreEqual(@"2013\2013-05", path);
		}

		[TestMethod]
		public void testYYYYMMDD()
		{
			var org = new DirOrganizerByYYYYMMDD();

			var path = org.GetDir(new FileContext { datetime = new DateTime(2013, 5, 12, 12, 21, 31, DateTimeKind.Local) });

			Assert.AreEqual(@"2013\2013-05\2013-05-12", path);
		}
	}
}
