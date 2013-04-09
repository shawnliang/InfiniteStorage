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
	public class TestTempFile
	{
		[TestMethod]
		public void testTempFile()
		{
			var f = new TempFile(".");
			Assert.IsTrue(!string.IsNullOrEmpty(f.Path));
			f.Write(new byte[] { 1, 2, 3 });
			f.Write(new byte[] { 4, 5 });
			f.EndWrite();

			Assert.AreEqual(5, f.BytesWritten);
			var fileInfo = new FileInfo(f.Path);
			Assert.AreEqual(5, fileInfo.Length);
		}
	}
}
