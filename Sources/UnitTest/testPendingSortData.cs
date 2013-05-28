using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace UnitTest
{
	class ddd
	{
		public Guid a { get; set; }
		public DateTime b { get; set; }
	}

	[TestClass]
	public class testPendingSortData
	{

		[TestMethod]
		public void TestGuid()
		{
			var data = "{\"a\": \"CE913009-5B93-491E-89F2-871B08FA665F\"}";
			var a = JsonConvert.DeserializeObject<ddd>(data);

			Assert.AreEqual(new Guid("CE913009-5B93-491E-89F2-871B08FA665F"), a.a);
		}

		[TestMethod]
		public void TestDateTime()
		{
			var data = "{\"b\": \"2010-10-20 20:30:40\"}";
			var b = JsonConvert.DeserializeObject<ddd>(data);

			Assert.AreEqual(new DateTime(2010, 10, 20, 20, 30, 40, DateTimeKind.Local), b.b);
		}
	}
}
