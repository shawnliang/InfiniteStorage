using System;
using System.Collections.Generic;
using System.Text;
using InfiniteStorage.Properties;
using System.Security.Cryptography;
using Waveface.Common;
using postServiceLibrary;

namespace InfiniteStorage.Cloud
{
	static class CloudService
	{
		static byte[] ENTROPY = { 25, 22, 89, 00, 77, 06, 25 };

		private static string _session_token = null;

		public static string UserEmail
		{
			get
			{
				var email = Settings.Default.UserEmail;

				if (string.IsNullOrEmpty(email))
				{
					email = Guid.NewGuid().ToString() + "@anonymous.waveface.com";
					Settings.Default.UserEmail = email;
					Settings.Default.Save();
				}

				return email;
			}
		}

		public static string Password
		{
			get
			{
				var password = Settings.Default.UserHash;

				if (string.IsNullOrEmpty(password))
				{
					password = randomPassword();

					var cipherBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(password), ENTROPY, DataProtectionScope.CurrentUser);
					var cipherText = HexConvert.ByteArrayToString(cipherBytes);

					Settings.Default.UserHash = cipherText;
					Settings.Default.Save();

					return password;
				}
				else
				{
					var cipherBytes = HexConvert.StringToByteArray(password);
					var bytes = ProtectedData.Unprotect(cipherBytes, ENTROPY, DataProtectionScope.CurrentUser);

					return Encoding.UTF8.GetString(bytes);
				}
			}
		}

		public static string SessionToken
		{
			get
			{
				if (string.IsNullOrEmpty(_session_token))
				{
					if (!Settings.Default.UserRegistered)
					{
						var post = new postServiceClass { APIKEY = APIKey };
						_session_token = post.createAccount(UserEmail, Password, "anonymous");

						_session_token = post.callLogin(UserEmail, Password);

						Settings.Default.GroupId = post.group_id;
						Settings.Default.UserRegistered = true;
						Settings.Default.Save();
					}
					else
					{
						var post = new postServiceClass { APIKEY = APIKey };
						_session_token = post.callLogin(UserEmail, Password);
					}
				}
				
				return _session_token;
			}
		}

		public static string APIKey
		{
			get { return "a23f9491-ba70-5075-b625-b8fb5d9ecd90"; }
		}

		public static postServiceClass CreateCloudAPI()
		{
			return new postServiceClass { APIKEY = APIKey, session_token = SessionToken, group_id = Settings.Default.GroupId };
		}

		private static string randomPassword()
		{
			var random = new Random();

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 10; i++)
			{
				char cur = (char)('a' + random.Next(26));

				if (random.Next(3) == 0)
					cur = Char.ToUpper(cur);

				sb.Append(cur);
			}

			return sb.ToString();
		}

	}

}