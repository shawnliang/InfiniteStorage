﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage;
using System.IO;
using InfiniteStorage.WebsocketProtocol;

namespace UnitTest
{
	[TestClass]
	public class testFileMover
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
			var mover = new FileMover();
			var newName = mover.Move("temp.jpg", @"dev\a.jpg");

			Assert.AreEqual(@"dev\a.1.jpg", newName);

			Assert.IsTrue(File.Exists(@"dev\a.1.jpg"));
		}
	}
}