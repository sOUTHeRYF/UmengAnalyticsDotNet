using System;
using System.Xml;
using System.Xml.Linq;

namespace UmengSDK.Common
{
	internal class AppInfo
	{
		public static string GetVersion()
		{
			try
			{
				XDocument xDocument = XDocument.Load("WMAppManifest.xml");
				if (xDocument != null)
				{
					using (XmlReader xmlReader = xDocument.CreateReader(0))
					{
						xmlReader.ReadToDescendant("App");
						if (xmlReader.IsStartElement())
						{
							return xmlReader.GetAttribute("Version");
						}
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("Fail to read App version from WMAppManifest ", e);
			}
			return "unknown";
		}

		public static string GetProductId()
		{
			try
			{
				XDocument xDocument = XDocument.Load("WMAppManifest.xml");
				if (xDocument != null)
				{
					using (XmlReader xmlReader = xDocument.CreateReader(0))
					{
						xmlReader.ReadToDescendant("App");
						if (xmlReader.IsStartElement())
						{
							return xmlReader.GetAttribute("ProductID");
						}
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("Fail to read App ProductID from WMAppManifest ", e);
			}
			return "unknown";
		}
	}
}
