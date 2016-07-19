using System;
using System.Diagnostics;
using System.Reflection;

namespace UmengSDK.Common
{
	internal class DebugUtil
	{
		private static void Logg(string info, string prefix = "udebug----------->")
		{
			if (!Constants.IsDebug || string.IsNullOrEmpty(info))
			{
				return;
			}
			try
			{
				MethodInfo method = typeof(Debug).GetMethod("WriteLine", new Type[]
				{
					typeof(string)
				});
				method.Invoke(null, new object[]
				{
					prefix + info
				});
			}
			catch (Exception)
			{
			}
		}

		public static void Log(Exception e)
		{
			if (e != null)
			{
				DebugUtil.Log(e.Message + "\n" + e.StackTrace, "udebug----------->");
			}
		}

		public static void Log(string info, Exception e)
		{
			DebugUtil.Log(info, "udebug----------->");
			DebugUtil.Log(e);
		}

		public static void Log(string s, string prefix = "udebug----------->")
		{
			if (s == null)
			{
				return;
			}
			if (s.Length <= 500)
			{
				DebugUtil.Logg(s, prefix);
				return;
			}
			if (s.Length <= 1000)
			{
				DebugUtil.Logg(s.Substring(0, 500), prefix);
				DebugUtil.Logg(s.Substring(500, s.Length - 500), prefix);
				return;
			}
			if (s.Length <= 1500)
			{
				DebugUtil.Logg(s.Substring(0, 500), prefix);
				DebugUtil.Logg(s.Substring(500, 500), prefix);
				DebugUtil.Logg(s.Substring(1000, s.Length - 1000), prefix);
				return;
			}
			int num = 0;
			while (num + 500 < s.Length)
			{
				DebugUtil.Logg(s.Substring(num, 500), prefix);
				num += 500;
			}
			DebugUtil.Logg(s.Substring(num), prefix);
		}
	}
}
