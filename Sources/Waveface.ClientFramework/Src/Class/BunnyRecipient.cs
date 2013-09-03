using System;

namespace Waveface.ClientFramework
{
	public class BunnyRecipient
	{
		public string Email { get; private set; }
		public string Name { get; private set; }

		public BunnyRecipient()
		{
			Email = Name = string.Empty;
		}

		public BunnyRecipient(string email, string name)
		{
			Email = email;
			Name = name;
		}

		public override bool Equals(object obj)
		{
			if (Object.ReferenceEquals(obj, this))
				return true;

			var rhs = obj as BunnyRecipient;
			if (rhs == null)
				return false;

			return Email == rhs.Email && Name == rhs.Name;
		}

		public override int GetHashCode()
		{
			return Email.GetHashCode() + Name.GetHashCode();
		}
	}
}
