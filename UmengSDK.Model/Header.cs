using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows;
using UmengSDK.Business;
using UmengSDK.Common;

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
			base.put(this.KEY_DEVICE, this.getDeviceID());
			base.put(this.KEY_ANID2, Header.getUserID());
			base.put(this.KEY_DEVICE_MODEL, string.Format("{0}_{1}", DeviceStatus.get_DeviceManufacturer(), DeviceStatus.get_DeviceName()));
			this.getResolution();
			base.put(this.KEY_SDK_TYPE, Constants.SDK_TYPE);
			base.put(this.KEY_SDK_VERSION, Constants.SDK_VERSION);
			base.put(this.KEY_OS, Constants.OS);
			base.put(this.KEY_OS_VERSION, Environment.get_OSVersion().get_Version().ToString());
			base.put(this.KEY_APP_VERSION, AppInfo.GetVersion());
			base.put(this.KEY_PACKAGE, AppInfo.GetProductId());
			this.getCulture();
			base.put(this.KEY_TIMEZONE, this.GetDSTAdjustedTimeZone());
			base.put(this.KEY_CARRIER, DeviceNetworkInformation.get_CellularMobileOperator());
			this.getNetworkType();
			this.getNetworkName();
		}

		private string getIDMD5()
		{
			try
			{
				string text = UmengSettings.Get<string>(this.KEY_DEVICE_ID, null);
				if (string.IsNullOrEmpty(text))
				{
					byte[] array = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
					if (array != null && array.Length > 0)
					{
						text = MD5Core.GetHashString(array);
						if (!string.IsNullOrEmpty(text))
						{
							UmengSettings.Put(this.KEY_DEVICE_ID, text);
						}
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

		public string getDeviceID()
		{
			try
			{
				string text = UmengSettings.Get<string>(this.KEY_DEVICE, null);
				if (string.IsNullOrEmpty(text))
				{
					byte[] array = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
					if (array != null && array.Length > 0)
					{
						text = BitConverter.ToString(array).Replace("-", "");
						if (!string.IsNullOrEmpty(text))
						{
							UmengSettings.Put(this.KEY_DEVICE, text);
						}
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

		public static string getUserID()
		{
			try
			{
				return UserExtendedProperties.GetValue("ANID2") as string;
			}
			catch (Exception e)
			{
				DebugUtil.Log("maybe missing permission ID_CAP_IDENTITY_DEVICE", e);
			}
			return "1234567890";
		}

		private void getResolution()
		{
			AutoResetEvent myResetEvent = new AutoResetEvent(false);
			Deployment.get_Current().get_Dispatcher().BeginInvoke(delegate
			{
				try
				{
					DebugUtil.Log("get resolution", "udebug----------->");
					this.put(this.KEY_RESOLUTION, this.GetResolutionWP8());
				}
				catch (Exception ex)
				{
					DebugUtil.Log("Fail to read resolution :" + ex.get_Message(), "udebug----------->");
				}
				finally
				{
					myResetEvent.Set();
				}
			});
			myResetEvent.WaitOne(2000);
		}

		private string GetResolutionWP8()
		{
			int scaleFactor = Application.get_Current().get_Host().get_Content().get_ScaleFactor();
			if (scaleFactor == 100)
			{
				return "480*800";
			}
			if (scaleFactor == 150)
			{
				return "720*1280";
			}
			if (scaleFactor != 160)
			{
				return "unknown";
			}
			return "768*1280";
		}

		private void getCulture()
		{
			try
			{
				string text = CultureInfo.get_CurrentCulture().ToString();
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
				int num = (int)TimeZoneInfo.get_Local().get_BaseUtcOffset().get_TotalHours();
				if (TimeZoneInfo.get_Local().IsDaylightSavingTime(DateTime.get_Now()))
				{
					num++;
				}
				result = num;
			}
			catch (Exception e)
			{
				DebugUtil.Log("Fail to read timezone (default 8) :", e);
				result = 8;
			}
			return result;
		}

		private void getNetworkType()
		{
			NetworkInterfaceType networkInterfaceType = NetworkInterface.get_NetworkInterfaceType();
			string value = networkInterfaceType.ToString();
			NetworkInterfaceType networkInterfaceType2 = networkInterfaceType;
			if (networkInterfaceType2 <= 6)
			{
				if (networkInterfaceType2 == 0 || networkInterfaceType2 == 6)
				{
					value = "unknown";
				}
			}
			else if (networkInterfaceType2 != 71)
			{
				switch (networkInterfaceType2)
				{
				case 145:
				case 146:
					value = "2G/3G";
					break;
				}
			}
			else
			{
				value = "Wi-Fi";
			}
			base.put(this.KEY_ACCESS, value);
		}

		public void getNetworkName()
		{
			DebugUtil.Log("get network name", "udebug----------->");
			AutoResetEvent myResetEvent = new AutoResetEvent(false);
			DeviceNetworkInformation.ResolveHostNameAsync(new DnsEndPoint("www.baidu.com", 80), delegate(NameResolutionResult handle)
			{
				try
				{
					NetworkInterfaceInfo networkInterface = handle.get_NetworkInterface();
					if (networkInterface != null)
					{
						NetworkInterfaceType interfaceType = networkInterface.get_InterfaceType();
						if (interfaceType != 6)
						{
							if (interfaceType != 71)
							{
								switch (interfaceType)
								{
								case 145:
								case 146:
									this.put(this.KEY_SUB_ACCESS, this.ConvertInterfaceSubtype(networkInterface.get_InterfaceSubtype()));
									break;
								default:
									this.put(this.KEY_ACCESS, "None");
									break;
								}
							}
							else
							{
								this.put(this.KEY_ACCESS, "WiFi");
							}
						}
						else
						{
							this.put(this.KEY_ACCESS, "Ethernet");
						}
					}
				}
				catch (Exception e)
				{
					DebugUtil.Log(e);
				}
				finally
				{
					myResetEvent.Set();
				}
			}, null);
			myResetEvent.WaitOne(5000);
		}

		private string ConvertInterfaceSubtype(NetworkInterfaceSubType subType)
		{
			string result = string.Empty;
			switch (subType)
			{
			case 1:
				result = "GPRS";
				break;
			case 2:
				result = "2G";
				break;
			case 3:
				result = "EVDO";
				break;
			case 4:
				result = "EDGE";
				break;
			case 5:
				result = "3G";
				break;
			case 6:
				result = "HSPA";
				break;
			case 7:
				result = "EVDV";
				break;
			default:
				result = "unknown";
				break;
			}
			return result;
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

		public Dictionary<string, object> ToDictionary()
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
				if (dictionary.get_Count() > 0)
				{
					base.put("uinfo", dictionary);
				}
			}
			return base.ToDictionary();
		}
	}
}
