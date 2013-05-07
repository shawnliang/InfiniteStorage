using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfiniteStorage;

namespace UnitTest
{
	/// <summary>
	/// Summary description for TestNormalizer
	/// </summary>
	[TestClass]
	public class TestNormalizer
	{

		[TestMethod]
		public void TestMethod1()
		{
			int a, b;

			Normalizer.NormalizeToInt(0xFFFFFFFFFL, 0xFFFFFFFFL, out a, out b);

			Assert.AreEqual((int)0x7fffffff, a);
			Assert.AreEqual(0x7FFFFFF, b);
		}

		[TestMethod]
		public void TestMethod2()
		{
			int a, b;

			Normalizer.NormalizeToInt(1024 * 1024, 1024 * 1023, out a, out b);

			Assert.AreEqual(1024*1024, a);
			Assert.AreEqual(1024*1023, b);
		}
	}
}
