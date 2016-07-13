using System;
using System.Collections.Generic;
using UmengSDK.Common;
using UmengSDK.Model;

namespace UmengSDK.Business
{
	internal class MessageBuilder
	{
		public static string buildLogMessage(Launch launch, Terminate terminate, Body body)
		{
			try
			{
				if (body != null)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					if (launch != null)
					{
						body.addLaunchSession(launch);
					}
					if (terminate != null)
					{
						body.addTerminalSession(terminate);
					}
					dictionary.Add("body", body.ToDictionary());
					dictionary.Add("header", Header.Instance().ToDictionary());
					return JSON.JsonEncode(dictionary);
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("build message error ", e);
			}
			return null;
		}

		public static string buildUpdateMessage()
		{
			string result;
			try
			{
				result = JSON.JsonEncode(Header.Instance().ToUpdateDictionary());
			}
			catch (Exception e)
			{
				DebugUtil.Log("build message error ", e);
				result = null;
			}
			return result;
		}

		public static string buildConfigMessage()
		{
			string result;
			try
			{
				Dictionary<string, object> dictionary = Header.Instance().ToOnlineConfigDictionary();
				dictionary.Add("last_config_time", OnlineConfigManager.Current.LastConfigTime);
				result = JSON.JsonEncode(dictionary);
			}
			catch (Exception e)
			{
				DebugUtil.Log("build message error ", e);
				result = null;
			}
			return result;
		}

		public static string buildParamMessage(string lastUpdateTime)
		{
			string result;
			try
			{
				Dictionary<string, object> dictionary = Header.Instance().ToOnlineConfigDictionary();
				dictionary.Add("last_config_time", lastUpdateTime);
				result = JSON.JsonEncode(dictionary);
			}
			catch (Exception e)
			{
				DebugUtil.Log("build message error ", e);
				result = null;
			}
			return result;
		}

		public static string getCurrentMessage(Body body)
		{
			string result;
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("body", body.ToDictionary());
				dictionary.Add("header", Header.Instance().ToDictionary());
				result = JSON.JsonEncode(dictionary);
			}
			catch (Exception e)
			{
				DebugUtil.Log("build message error ", e);
				result = null;
			}
			return result;
		}
	}
}
