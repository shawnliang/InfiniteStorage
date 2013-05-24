using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage;

namespace UnitTest.Pending
{
	[TestClass]
	public class testEventDirOrganizer
	{
		[TestMethod]
		public void sanitizeTitle()
		{
			var org = new EventDirOrganizer();
			var folder = org.GetEventFolder(new PendingEvent
			{
				time_start = new DateTime(2012, 3, 4, 10, 2, 3),
				time_end = new DateTime(2012, 3, 5),
				title = "a/b\\c:d*e?f\"g>h<i|j"
			});


			Assert.AreEqual(@"a_b_c_d_e_f_g_h_i_j", folder);
		}

		[TestMethod]
		public void create_folder_if_no_title()
		{
			var org = new EventDirOrganizer();
			var folder = org.GetEventFolder(new PendingEvent
			{
				time_start = new DateTime(2012, 3, 4, 10, 2, 3, DateTimeKind.Local),
				time_end = new DateTime(2012, 3, 5, 0,0,0, DateTimeKind.Local),
				title = null
			});


			Assert.AreEqual(@"2012-03-04 10-02-03 ~ 2012-03-05 00-00-00", folder);
		}
	}
}
