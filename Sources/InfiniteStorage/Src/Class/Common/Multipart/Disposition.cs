using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Wammer.MultiPart
{
	public class Disposition
	{
		private static readonly char[] SEPARATOR = new[] { ';' };
		private readonly NameValueCollection parameters = new NameValueCollection();
		private string value;

		public Disposition(string value)
		{
			this.value = value;
		}

		private Disposition()
		{
		}

		public string Value
		{
			get { return value; }
		}

		public NameValueCollection Parameters
		{
			get { return parameters; }
		}

		public static Disposition Parse(string text)
		{
			try
			{
				string[] segments = text.Split(SEPARATOR,
											   StringSplitOptions.RemoveEmptyEntries);

				var disp = new Disposition { value = segments[0].Trim() };
				for (int i = 1; i < segments.Length; i++)
				{
					string[] nameValue = segments[i].Split('=');

					string paramValue = RemoveDoubleQuote(nameValue[1].Trim());
					disp.parameters.Add(nameValue[0].Trim(), paramValue);
				}

				return disp;
			}
			catch (Exception e)
			{
				throw new FormatException(
					"Incorrect content disposition format: " + text, e);
			}
		}

		public static string RemoveDoubleQuote(string str)
		{
			if (str.StartsWith("\"") && str.EndsWith("\""))
				return str.Substring(1, str.Length - 2);
			return str;
		}

		public void CopyTo(Stream output)
		{
			var buff = new StringBuilder();
			buff.Append("Content-Disposition: ");
			buff.Append(value);
			if (parameters.Count > 0)
			{
				foreach (string key in parameters.AllKeys)
				{
					buff.Append(";");
					buff.Append(key);
					buff.Append("=");
					buff.Append("\"");
					buff.Append(parameters[key]);
					buff.Append("\"");
				}
			}
			buff.Append("\r\n");

			byte[] data = Encoding.UTF8.GetBytes(buff.ToString());
			output.Write(data, 0, data.Length);
		}
	}
}