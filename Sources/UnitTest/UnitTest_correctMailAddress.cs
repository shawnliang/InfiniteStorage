using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wpf_testHTTP;

namespace UnitTest
{
	[TestClass]
	public class UnitTest_correctMailAddress
	{

		[TestMethod]
		[Ignore]
		public void TestMethod1()
		{
			bool _result = false;
			string _phone = "Kimi.Chiu@waveface.com";
			Wpf_testHTTP.MainWindow _sharePhoto = new MainWindow();
			_result = _sharePhoto.IsValidEmail(_phone);
			Assert.IsTrue(_result);

			_phone = "Kimi.Chiu@waveface";
			_result = _sharePhoto.IsValidEmail(_phone);
			Assert.IsFalse(_result);
		}
	}
}
