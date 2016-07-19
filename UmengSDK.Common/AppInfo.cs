using System;
using System.Xml;
using System.Xml.Linq;

namespace UmengSDK.Common
{
	internal class AppInfo
	{
		public static string GetVersion()
		{
            return string.IsNullOrEmpty(UmengAnalytics.AppVersion) ? "unknown" : UmengAnalytics.AppVersion;            
		}

		public static string GetProductId()
		{
            return string.IsNullOrEmpty(UmengAnalytics.PackageName) ? "unknown" : UmengAnalytics.PackageName;
        }
	}
}
