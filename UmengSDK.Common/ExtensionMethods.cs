using System;

namespace UmengSDK.Common
{
	internal static class ExtensionMethods
	{
		public static string CheckInput(this string s, int maxLength = 256)
		{
			string text = s;
			try
			{
				if (!string.IsNullOrEmpty(text) && text.get_Length() > maxLength)
				{
					string text2 = text;
					text = text.Substring(0, maxLength);
					DebugUtil.Log(string.Format("{0} beyond the max lenth({1}), truncate it to {2}", text2, maxLength, text), "udebug----------->");
				}
			}
			catch (Exception)
			{
				text = s;
			}
			return text;
		}
	}
}
