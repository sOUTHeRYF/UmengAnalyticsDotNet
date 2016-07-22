using System;
using System.Xml;
using System.Xml.Linq;

using UmengSDK.Business;
namespace UmengSDK.Common
{
	internal class AppInfo
	{
		public static string GetVersion()
		{
            return string.IsNullOrEmpty(Manager.appVersion)?"unknown" : Manager.appVersion;            
		}

		public static string GetProductId()
		{
            return string.IsNullOrEmpty(Manager.packageName) ? "unknown" : Manager.packageName;
        }
	}
}
