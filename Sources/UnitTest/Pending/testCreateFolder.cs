using InfiniteStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTest.Pending
{
	[TestClass]
	public class testCreateFolder
	{
		private PendingToResourceUtil util;

		[TestInitialize]
		public void setup()
		{
			util = new PendingToResourceUtil("Data Source=db", "dev_folder", Path.Combine(Environment.CurrentDirectory, "res"));

			if (Directory.Exists("res"))
				Directory.Delete("res", true);
		}


		[TestMethod]
		public void return_relative_path()
		{
			var result = util.CreateFolder(@"2010\2010-10\title");
			Assert.AreEqual(@"2010\2010-10\title", result);
		}

		[TestMethod]
		public void create_that_folder()
		{
			var result = util.CreateFolder(@"2010\2010-10\title");

			var dirCreated = Directory.Exists(Path.Combine("res", "dev_folder", result));
			Assert.IsTrue(dirCreated);
		}
	}
}
