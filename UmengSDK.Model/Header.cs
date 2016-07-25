//using Microsoft.Phone.Info;
//using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Management;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows;
using UmengSDK.Business;
using UmengSDK.Common;
using System.Windows.Forms;
namespace UmengSDK.Model
{
	internal class Header : Hub
	{
		private string KEY_APPKEY = "appkey";

		private string KEY_DEVICE_ID = "device_id";

		private string KEY_DEVICE_MODEL = "device_model";

		private string KEY_DEVICE = "wp_device";

		private string KEY_ANID2 = "wp_anid2";

		private string KEY_RESOLUTION = "resolution";

		private string KEY_SDK_TYPE = "sdk_type";

		private string KEY_SDK_VERSION = "sdk_version";

		private string KEY_OS = "os";

		private string KEY_OS_VERSION = "os_version";

		private string KEY_APP_VERSION = "app_version";

		private string KEY_COUNTRY = "country";

		private string KEY_LANGUAGE = "language";

		private string KEY_TIMEZONE = "timezone";

		private string KEY_ACCESS = "access";

		private string KEY_SUB_ACCESS = "access_subtype";

		private string KEY_CARRIER = "carrier";

		private string KEY_CHANNEL = "channel";

		private string KEY_PACKAGE = "package";

		private string KEY_CURRENT_VERSION = "current_version";

		private static volatile Header _instance = null;

		private static readonly object lockHelper = new object();

		public static Header Instance()
		{
			if (Header._instance == null)
			{
				lock (Header.lockHelper)
				{
					if (Header._instance == null)
					{
						Header._instance = new Header();
					}
				}
			}
			return Header._instance;
		}

		private Header()
		{
			try
			{
				this.initBasicInfo();
			}
			catch (Exception e)
			{
				DebugUtil.Log("init Header error :", e);
			}
		}

		private void initBasicInfo()
		{
			base.put(this.KEY_APPKEY, Manager.AppKey);
			base.put(this.KEY_CHANNEL, Manager.Channel);
			base.put(this.KEY_DEVICE_ID, this.getIDMD5());
            string deviceID = getDeviceID();
            base.put(this.KEY_DEVICE, deviceID);
			base.put(this.KEY_ANID2, string.IsNullOrEmpty(Header.getUserID())?deviceID:Header.getUserID());
			base.put(this.KEY_DEVICE_MODEL, getComputerName());
			this.getResolution();
			base.put(this.KEY_SDK_TYPE, Constants.SDK_TYPE);
			base.put(this.KEY_SDK_VERSION, Constants.SDK_VERSION);
			base.put(this.KEY_OS, Constants.OS);
			base.put(this.KEY_OS_VERSION, Environment.OSVersion.Version.ToString());
			base.put(this.KEY_APP_VERSION, AppInfo.GetVersion());
			base.put(this.KEY_PACKAGE, AppInfo.GetProductId());
			this.getCulture();
			base.put(this.KEY_TIMEZONE, this.GetDSTAdjustedTimeZone());
			base.put(this.KEY_CARRIER, "unknown");
			this.getNetworkType();
			this.getNetworkName();
		}
        private string getComputerName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "unknow";
            }
        }
		private string getIDMD5()
		{
         //   return "1234567895461245";
			try
			{
				string text = UmengSettings.Get<string>(this.KEY_DEVICE_ID, null);
				if (string.IsNullOrEmpty(text))
				{
                    string mac = "";
                    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            mac = mo["MacAddress"].ToString();
                            break;
                        }
                    }
                    moc = null;
                    mc = null;
                    text = MD5Core.GetHashString(mac);
				    if (!string.IsNullOrEmpty(text))
					{
						UmengSettings.Put(this.KEY_DEVICE_ID, text);
					}
				}
				return text;
			}
			catch (Exception e)
			{
				DebugUtil.Log("maybe missing permission ID_CAP_IDENTITY_DEVICE", e);
			}
			return "1234567890";
		}

		public  string getDeviceID()
		{
       //     return "1234567895461245";

            try
			{
				string text = UmengSettings.Get<string>(this.KEY_DEVICE, null);
				if (string.IsNullOrEmpty(text))
				{
                    string mac = "";
                    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            mac = mo["MacAddress"].ToString();
                            break;
                        }
                    }
                    moc = null;
                    mc = null;
                    UmengSettings.Put(this.KEY_DEVICE, mac);
                    return mac;
				}
				return text;
			}
			catch (Exception e)
			{
				DebugUtil.Log("maybe missing permission ID_CAP_IDENTITY_DEVICE", e);
			}
			return "1234567890";
		}

		public static string getUserID()
		{
            return Manager.userID;

        }

		private void getResolution()
		{
            int SH = Screen.PrimaryScreen.Bounds.Height;
            int SW = Screen.PrimaryScreen.Bounds.Width;
            this.put(this.KEY_RESOLUTION, SW.ToString()+"*"+SH.ToString());
				
		}
		private void getCulture()
		{
			try
			{
				string text = CultureInfo.CurrentCulture.ToString();
				string[] array = text.Split(new char[]
				{
					'-'
				});
				if (array != null && array.Length == 2)
				{
					base.put(this.KEY_LANGUAGE, array[0]);
					base.put(this.KEY_COUNTRY, array[1]);
					return;
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("Fail to read culture :", e);
			}
			base.put(this.KEY_LANGUAGE, "unknown");
			base.put(this.KEY_COUNTRY, "unknown");
		}

		private int GetDSTAdjustedTimeZone()
		{
            int result;
            try
            {
                int num = 0;
                int.TryParse(TimeZoneInfo.Local.DisplayName.Split(new char[]
                {
                    ' '
                })[0].Split(new char[]
                {
                    ':'
                })[0].Substring(3), out num);
                if (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now))
                {
                    num++;
                }
                result = Math.Abs(num);
            }
            catch (Exception ex)
            {
           //     Debugger.Log("Fail to read timezone (default 8) :" + ex.Message);
                result = 8;
            }
            return result;
        }

		private void getNetworkType()
		{
            string value = "Wi-Fi";
			base.put(this.KEY_ACCESS, value);
		}

		public void getNetworkName()
		{
            this.put(this.KEY_ACCESS, "WiFi");
        }

		public Dictionary<string, object> ToOnlineConfigDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("type", "online_config");
			dictionary.Add(this.KEY_APPKEY, base.get(this.KEY_APPKEY));
			dictionary.Add(this.KEY_SDK_TYPE, Constants.SDK_TYPE_CONFIG);
			dictionary.Add(this.KEY_SDK_VERSION, base.get(this.KEY_SDK_VERSION));
			dictionary.Add(this.KEY_CHANNEL, base.get(this.KEY_CHANNEL));
			dictionary.Add(this.KEY_DEVICE_ID, base.get(this.KEY_DEVICE_ID));
			dictionary.Add(this.KEY_APP_VERSION, base.get(this.KEY_APP_VERSION));
			return dictionary;
		}

		public Dictionary<string, object> ToUpdateDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("type", "update");
			dictionary.Add(this.KEY_APPKEY, base.get(this.KEY_APPKEY));
			dictionary.Add(this.KEY_SDK_VERSION, base.get(this.KEY_SDK_VERSION));
			dictionary.Add(this.KEY_CHANNEL, base.get(this.KEY_CHANNEL));
			dictionary.Add(this.KEY_DEVICE_ID, base.get(this.KEY_DEVICE_ID));
			dictionary.Add(this.KEY_CURRENT_VERSION, base.get(this.KEY_APP_VERSION));
			dictionary.Add(this.KEY_SDK_TYPE, base.get(this.KEY_SDK_TYPE));
			return dictionary;
		}

		public new Dictionary<string, object> ToDictionary()
		{
			if (Constants.get_user_info)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				int num = UmengSettings.Get<int>(Constants.Key_Age, -1);
				int num2 = UmengSettings.Get<int>(Constants.Key_Gender, -1);
				string text = UmengSettings.Get<string>(Constants.Key_UserId, null);
				string text2 = UmengSettings.Get<string>(Constants.Key_Source, null);
				if (num > 0)
				{
					dictionary.Add("age", num);
				}
				if (num2 > 0)
				{
					dictionary.Add("sex", num2);
				}
				if (!string.IsNullOrEmpty(text))
				{
					dictionary.Add("id", text);
					if (!string.IsNullOrEmpty(text2))
					{
						dictionary.Add("source", text2);
					}
				}
				if (dictionary.Count > 0)
				{
					base.put("uinfo", dictionary);
				}
			}
			return base.ToDictionary();
		}
	}
}
