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
			if (!Directory.Exists("dev"))
				Directory.CreateDirectory("dev");

			using (var f = new StreamWriter(@"dev\a.jpg"))
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
			Assert.IsTrue(File.Exists(@"dev\a.jpg"));
			var flatFileStorage = new FlatFileStorage(".", ".", ".");
			flatFileStorage.setDeviceName("dev");


			flatFileStorage.MoveToStorage("temp.jpg", "a.jpg");
			Assert.IsTrue(File.Exists(@"dev\a.jpg"));
			Assert.IsTrue(File.Exists(@"dev\a.1.jpg"));

			using (var f = new StreamReader(@"dev\a.1.jpg"))
			{
				Assert.AreEqual("temp", f.ReadToEnd());
			}
		}
	}
}
