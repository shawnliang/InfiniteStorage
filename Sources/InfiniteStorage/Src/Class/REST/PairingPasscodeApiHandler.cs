using Wammer.Station;

namespace InfiniteStorage.REST
{
	class PairingPasscodeApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			var reply = new
			{
				status = 200,
				api_ret_code = 0,
				api_ret_message = "success",
				passcode = BonjourServiceRegistrator.Instance.Passcode
			};

			respondSuccess(reply);
		}
	}
}
