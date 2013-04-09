using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage;
using System.IO;

namespace UnitTest
{
	[TestClass]
	public class testFlatFileStorage
	{
		[TestInitialize]
		public void setup()
		{
			using (var f = new StreamWriter("a.jpg"))
			{
				f.Write("aaa");
			}

			using (var f = new StreamWriter("temp.jpg"))
			{
				f.Write("temp");
			}
		}

		[TestMethod]
		public void appendNumsToDupFileName()
		{
			Assert.IsTrue(File.Exists("a.jpg"));
			var flatFileStorage = new FlatFileStorage(".");

			flatFileStorage.MoveToStorage("temp.jpg", "a.jpg");
			Assert.IsTrue(File.Exists("a.jpg"));
			Assert.IsTrue(File.Exists("a.1.jpg"));

			using (var f = new StreamReader("a.1.jpg"))
			{
				Assert.AreEqual("temp", f.ReadToEnd());
			}
		}
	}
}
