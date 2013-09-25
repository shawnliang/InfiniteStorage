#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

#endregion

namespace Wammer.MultiPart
{
	public class Parser
	{
		private static readonly byte[] CRLF = Encoding.UTF8.GetBytes("\r\n");
		private static readonly byte[] DCRLF = Encoding.UTF8.GetBytes("\r\n\r\n");
		private static readonly int[] DCRLF_next = Next(DCRLF);

		private readonly byte[] m_headBoundry;
		private readonly int[] m_headBoundryNext;

		public Parser(string boundry)
		{
			m_headBoundry = Encoding.UTF8.GetBytes("--" + boundry);
			m_headBoundryNext = Next(m_headBoundry);
		}

		public Part[] Parse(byte[] content)
		{
			var parts = new List<Part>();

			int startFrom = IndexOf(content, 0, m_headBoundry, m_headBoundryNext);

			if (startFrom != -1)
			{
				startFrom += m_headBoundry.Length + CRLF.Length;
				bool end = false;

				while (!end)
				{
					Part part = ParsePartBody(content, startFrom, out startFrom, out end);
					parts.Add(part);
				}

				return parts.ToArray();
			}

			throw new FormatException("Not a wellformed multipart content");
		}

		private Part ParsePartBody(byte[] data, int startIdx, out int next_startIdx, out bool end)
		{
			var headers = new NameValueCollection();

			int sep_index;

			if (data[startIdx] == '\r' && data[startIdx + 1] == '\n')
			{
				// no header in this part, sep_index is right after boundary
				//
				// ---boundary\r\n
				// \r\n
				// data from here....
				sep_index = startIdx - CRLF.Length;
			}
			else
			{
				sep_index = IndexOf(data, startIdx, DCRLF, DCRLF_next);
				ParseHeaders(headers, data, startIdx, sep_index - startIdx);
			}

			int next_head_index = IndexOf(data, startIdx, m_headBoundry, m_headBoundryNext);
			next_startIdx = next_head_index + m_headBoundry.Length;

			if (next_head_index < 0 || next_startIdx + CRLF.Length > data.Length)
				throw new FormatException("Bad part body format");

			end = false;

			// cheat on looking close_boundary & following \r\n
			if (data[next_startIdx] == '-' && data[next_startIdx + 1] == '-')
			{
				end = true;
			}
			else if (data[next_startIdx] == '\r' && data[next_startIdx + 1] == '\n')
			{
				next_startIdx += CRLF.Length;
			}
			else
			{
				throw new FormatException("Bad part body format");
			}

			return
				new Part(
					new ArraySegment<byte>(data, sep_index + DCRLF.Length, next_head_index - (sep_index + DCRLF.Length) - CRLF.Length),
					headers);
		}

		private static void ParseHeaders(NameValueCollection collection, byte[] data, int from, int len)
		{
			string headerText = Encoding.UTF8.GetString(data, from, len);
			var stringSeparators = new[] { "\r\n" };
			string[] headers = headerText.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

			foreach (string header in headers)
			{
				var delimitIdx = header.IndexOf(":");

				if (delimitIdx < 0)
					throw new FormatException("Bad header: " + header);

				string key = header.Substring(0, delimitIdx).Trim();
				string val = header.Substring(delimitIdx + 1).Trim();
				collection.Add(key, val);
			}
		}

		// KMP algorithm reference: http://www.cnblogs.com/zhy2002/archive/2008/03/31/1131794.html
		public static int[] Next(byte[] pattern)
		{
			var next = new int[pattern.Length];
			next[0] = -1;

			if (pattern.Length < 2)
			{
				return next;
			}

			next[1] = 0;
			int i = 2;
			int j = 0;

			while (i < pattern.Length)
			{
				if (pattern[i - 1] == pattern[j])
				{
					next[i++] = ++j;
				}
				else
				{
					j = next[j];

					if (j == -1)
					{
						next[i++] = ++j;
					}
				}
			}

			return next;
		}

		// find "what" in a byte array
		private static int IndexOf(byte[] source, int stardIdx, byte[] pattern, int[] next)
		{
			int i = 0;
			int j = 0;

			while (j < pattern.Length && i < source.Length - stardIdx)
			{
				if (source[stardIdx + i] == pattern[j])
				{
					i++;
					j++;
				}
				else
				{
					j = next[j];
					if (j == -1)
					{
						i++;
						j++;
					}
				}
			}

			return j < pattern.Length ? -1 : stardIdx + i - j;
		}
	}
}