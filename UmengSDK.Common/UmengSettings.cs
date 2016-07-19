using System;

namespace UmengSDK.Common
{
	internal class UmengSettings
	{
		private const string SETTINGS_NAME = "UmengSettings.xml";

		private static SerializableDictionary<string, object> _settingsDic = null;

		private static readonly object syncObj = new object();

		public static void Load()
		{
			try
			{
				lock (UmengSettings.syncObj)
				{
					UmengSettings._settingsDic = IsoPersistentHelper.Load<SerializableDictionary<string, object>>("UmengSettings.xml");
					if (UmengSettings._settingsDic == null)
					{
						UmengSettings._settingsDic = new SerializableDictionary<string, object>();
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void Save()
		{
			try
			{
				lock (UmengSettings.syncObj)
				{
					if (UmengSettings._settingsDic != null && UmengSettings._settingsDic.Count > 0)
					{
						UmengSettings._settingsDic.Save("UmengSettings.xml");
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static bool Contains(string key)
		{
			try
			{
				lock (UmengSettings.syncObj)
				{
					if (!string.IsNullOrEmpty(key))
					{
						return UmengSettings._settingsDic.ContainsKey(key);
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in put UmengSettings", e);
			}
			return false;
		}

		public static void Put(string key, object value)
		{
			try
			{
				lock (UmengSettings.syncObj)
				{
					if (!string.IsNullOrEmpty(key) && value != null)
					{
						if (UmengSettings._settingsDic.ContainsKey(key))
						{
							UmengSettings._settingsDic[key] = value;
						}
						else
						{
							UmengSettings._settingsDic.Add(key, value);
						}
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in put UmengSettings", e);
			}
		}

		public static void Clear(string key)
		{
			try
			{
				lock (UmengSettings.syncObj)
				{
					UmengSettings._settingsDic.Remove(key);
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in clear UmengSettings", e);
			}
		}

		public static T Get<T>(string key, T defaultValue)
		{
			try
			{
				if (string.IsNullOrEmpty(key))
				{
					T result = defaultValue;
					return result;
				}
				if (UmengSettings._settingsDic.ContainsKey(key))
				{
					T result = (T)((object)UmengSettings._settingsDic[key]);
					return result;
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in get<T> UmengSettings", e);
			}
			return default(T);
		}

		public static void Acc(string key, int delta)
		{
			try
			{
				lock (UmengSettings.syncObj)
				{
					if (UmengSettings._settingsDic.ContainsKey(key))
					{
						int num = (int)UmengSettings._settingsDic[key];
						UmengSettings._settingsDic[key] = num + delta;
					}
					else
					{
						UmengSettings._settingsDic.Add(key, delta);
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in getInt UmengSettings", e);
			}
		}

		public static void Acc(string key, string msg)
		{
			try
			{
				lock (UmengSettings.syncObj)
				{
					if (UmengSettings._settingsDic.ContainsKey(key))
					{
						string text = (string)UmengSettings._settingsDic[key];
						UmengSettings._settingsDic[key] = (text + ";" + msg);
					}
					else
					{
						UmengSettings._settingsDic.Add(key, msg);
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in getInt UmengSettings", e);
			}
		}
	}
}
