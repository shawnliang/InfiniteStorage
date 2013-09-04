using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using Waveface.Model;

namespace UnitTest
{
	[TestClass]
	public class testObserverableCollecitonSort
	{
		[TestMethod]
		public void sort0()
		{
			var col = new ObservableCollection<string>(new string[] { });
			col.Sort((x, y) => x.CompareTo(y));

			Assert.AreEqual(0, col.Count);
		}

		[TestMethod]
		public void sort1()
		{
			var col = new ObservableCollection<string>(new string[] { "b", "c", "d", "a" });


			col.Sort((x, y) => x.CompareTo(y));

			Assert.AreEqual("a", col[0]);
			Assert.AreEqual("b", col[1]);
			Assert.AreEqual("c", col[2]);
			Assert.AreEqual("d", col[3]);
		}

		[TestMethod]
		public void sort2()
		{
			var col = new ObservableCollection<string>(new string[] { "a", "b", "c", "d" });


			col.Sort((x, y) => x.CompareTo(y));

			Assert.AreEqual("a", col[0]);
			Assert.AreEqual("b", col[1]);
			Assert.AreEqual("c", col[2]);
			Assert.AreEqual("d", col[3]);
		}

		[TestMethod]
		public void sort3()
		{
			var col = new ObservableCollection<string>(new string[] { "b" });


			col.Sort((x, y) => x.CompareTo(y));

			Assert.AreEqual("b", col[0]);
		}

		[TestMethod]
		public void sort4()
		{
			var col = new ObservableCollection<string>(new string[] { "b", "a" });


			col.Sort((x, y) => x.CompareTo(y));

			Assert.AreEqual("a", col[0]);
			Assert.AreEqual("b", col[1]);
		}
	}
}
