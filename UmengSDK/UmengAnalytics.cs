//using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Windows;
using UmengSDK.Business;
using UmengSDK.Common;

namespace UmengSDK
{
	public class UmengAnalytics
	{
		private delegate void UpdateEventHandler(int statusCode, UpdateEventArgs e);

		private delegate void OnlineParamHandler(int statusCode, OnlineParamEventArgs e);

		private static bool _initSuccessed;

		private static int MAX_LENGTH_30;

		private static readonly Manager _manager;

		private static readonly UpdateManager _updateMgr;

		private static readonly OnlineParamManager _onlineParamMgr;

		private static event UmengAnalytics.UpdateEventHandler CheckUpdateCompleted;

		private static event UmengAnalytics.OnlineParamHandler UpdateOnlineParamCompleted;

		private static string NumberKey
		{
			get
			{
				return "__ct__";
			}
		}

		public static bool IsDebug
		{
			get
			{
				return Constants.IsDebug;
			}
			set
			{
				Constants.IsDebug = value;
			}
		}

		static UmengAnalytics()
		{
			UmengAnalytics._initSuccessed = true;
			UmengAnalytics.MAX_LENGTH_30 = 30;
			UmengAnalytics._manager = null;
			UmengAnalytics._updateMgr = null;
			UmengAnalytics._onlineParamMgr = null;
			try
			{
				UmengAnalytics._manager = new Manager();
				UmengAnalytics._updateMgr = new UpdateManager();
				UmengAnalytics._onlineParamMgr = new OnlineParamManager();
				UmengAnalytics._onlineParamMgr.UpdateCompletedEvent = new OnlineParamManager.UpdateCompletedHandler(UmengAnalytics.OnlineParamCallback);
				UmengAnalytics._updateMgr.CheckCompletedEvent = new UpdateManager.CheckCompletedHandler(UmengAnalytics.OnCheckUpdateCompleted);
			}
			catch (Exception e)
			{
				UmengAnalytics._initSuccessed = false;
				DebugUtil.Log("UmengAnalytics constructor failed!", e);
			}
		}
        /// <summary>
        /// 初始化SDK
        /// </summary>
        /// <param name="appkey">后台申请的AppKey</param>
        /// <param name="packageName">包名，游戏唯一性</param>
        /// <param name="appVersion">版本，用于后台区分，格式为1.2.3.4</param>
        /// <param name="userID">用户ID，为空则使用设备ID</param>
        /// <param name="channel">渠道，为空则使用"Steam"</param>
		public static void Init(string appkey, string packageName,string appVersion,string userID = "",string channel = "Steam")
		{
			try
			{
				if (UmengAnalytics._initSuccessed)
				{
					if (!UmengAnalytics.IsValidAppkey(appkey))
					{
						DebugUtil.Log("Invalid app key!", "udebug----------->");
					}
					else
					{
                        if (!string.IsNullOrEmpty(channel))
                        {
                            channel = channel.CheckInput(UmengAnalytics.MAX_LENGTH_30);
                            Manager.Channel = channel;
                        }
                        else
                        {
                            Manager.Channel = "Steam";
                        }
                        Manager.packageName = packageName;
                        Manager.appVersion = appVersion;
                        Manager.userID = userID;           
						Manager.AppKey = appkey;
						UmengSettings.Load();
						UmengAnalytics.RegisterEvents();
						DebugUtil.Log("UmengAnalytics Init completed!", "udebug----------->");
					}
				}
			}
			catch (Exception e)
			{
				UmengAnalytics._initSuccessed = false;
				DebugUtil.Log("Umeng SDK init failed!", e);
			}
		}
        public static void StartTrack()
        {
            UpdateOnlineParamAsync();
            OnLaunching();
        }
        public static void EndTrack()
        {
            OnClosing();
        }
		private static void TrackPageStart(string pageName)
		{
			try
			{
				if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(pageName))
				{
					pageName = pageName.CheckInput(256);
					UmengAnalytics._manager.AddPageViewStart(pageName);
					DebugUtil.Log(string.Format("TrackPageStart : {0}", pageName), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		private static void TrackPageEnd(string pageName)
		{
			try
			{
				if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(pageName))
				{
					pageName = pageName.CheckInput(256);
					UmengAnalytics._manager.AddPageViewEnd(pageName);
					DebugUtil.Log(string.Format("TrackPageEnd : {0}", pageName), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void TrackEvent(string eventId)
		{
			try
			{
				if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(eventId))
				{
					UmengAnalytics._manager.AddEvent(eventId);
					DebugUtil.Log(string.Format("TrackEvent : {0}", eventId), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void TrackEvent(string eventId, string label)
		{
			try
			{
				if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(eventId) && UmengAnalytics.IsValidInput(label))
				{
					UmengAnalytics._manager.AddEvent(eventId, label);
					DebugUtil.Log(string.Format("TrackEvent - id:{0} label:{1}", eventId, label), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void TrackEvent(string eventId, long duration)
		{
			try
			{
				if (duration <= 0L)
				{
					DebugUtil.Log(string.Format("duration can't be less than or equal to 0:{0}", duration), "udebug----------->");
				}
				else if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(eventId))
				{
					UmengAnalytics._manager.AddEvent(eventId, duration);
					DebugUtil.Log(string.Format("TrackEventDuration - id:{0} duration:{1}", eventId, duration), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void TrackEvent(string eventId, string label, long duration)
		{
			try
			{
				if (duration <= 0L)
				{
					DebugUtil.Log(string.Format("duration can't be less than or equal to 0:{0}", duration), "udebug----------->");
				}
				else if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(eventId) && UmengAnalytics.IsValidInput(label))
				{
					UmengAnalytics._manager.AddEvent(eventId, label, duration);
					DebugUtil.Log(string.Format("TrackEventDuration - id:{0} label:{1} duration:{2}", eventId, label, duration), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void TrackEvent(string eventId, Dictionary<string, string> kv)
		{
			try
			{
				if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(eventId) && kv != null && kv.Count > 0)
				{
					UmengAnalytics._manager.AddEvent(eventId, kv);
					DebugUtil.Log(string.Format("TrackEvent - id:{0} kv:{1}", eventId, kv.ToString()), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void TrackEvent(string eventId, Dictionary<string, string> kv, long duration)
		{
			try
			{
				if (duration <= 0L)
				{
					DebugUtil.Log(string.Format("duration can't be less than or equal to 0:{0}", duration), "udebug----------->");
				}
				else if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(eventId) && kv != null && kv.Count > 0)
				{
					UmengAnalytics._manager.AddEvent(eventId, kv, duration);
					DebugUtil.Log(string.Format("TrackEventDuration - id:{0} duration:{1} kv:{2}", eventId, duration, kv.ToString()), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		private static void TrackError(string error)
		{
			try
			{
				if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(error))
				{
					UmengAnalytics._manager.AddError(error);
					DebugUtil.Log(string.Format("TrackError - {0}", error), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void TrackException(Exception ex, string errMsg = "")
		{
			try
			{
				if (UmengAnalytics._initSuccessed && UmengAnalytics.IsValidInput(ex))
				{
					UmengAnalytics._manager.AddError(ex, errMsg);
					DebugUtil.Log(string.Format("TrackException - {0}", ex.Message), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static void UpdateOnlineParamAsync()
		{
			try
			{
				if (UmengAnalytics._onlineParamMgr != null && UmengAnalytics.IsValidAppkey(Manager.AppKey))
				{
					UmengAnalytics._onlineParamMgr.Update();
				}
				else
				{
					DebugUtil.Log("UpdateOnlineParamAsync:AppKey can't be null, please call Init() first!", "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public static string GetOnlineParam(string key)
		{
            string result = string.Empty;
			try
			{
				if (UmengAnalytics._onlineParamMgr != null && UmengAnalytics.IsValidInput(key))
				{
					result = UmengAnalytics._onlineParamMgr.GetParam(key);
				}
				else
				{
					result = string.Empty;
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
				result = string.Empty;
			}
			return result;
		}

		private static void CheckUpdateAsync()
		{
			try
			{
				if (UmengAnalytics._updateMgr != null && UmengAnalytics.IsValidAppkey(Manager.AppKey))
				{
					UmengAnalytics._updateMgr.CheckUpdate();
				}
				else
				{
					DebugUtil.Log("CheckUpdateAsync:AppKey can't be null, please call Init() first!", "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		private static void SetSessionInterval(long seconds)
		{
			Constants.SessionInterval = seconds;
			DebugUtil.Log(string.Format("Session Interval changed to {0}s", seconds), "udebug----------->");
		}

		private static void DoneAppTerminate()
		{
			if (UmengAnalytics._manager != null)
			{
				UmengAnalytics._manager.OnPause();
			}
		}

		private static void EnableTrackLocation(bool enabled)
		{
			Constants.locationEnabled = enabled;
		}

		private static void OnLaunching()
		{
			try
			{
				UmengAnalytics.SetBodyFileName();
				UmengAnalytics._manager.OnResume();
				DebugUtil.Log("OnLaunching Completed", "udebug----------->");
			}
			catch (Exception e2)
			{
				DebugUtil.Log("OnLaunching failed!", e2);
			}
		}


		private static void OnClosing()
		{
			try
			{
				UmengAnalytics._manager.OnPause();
				UmengSettings.Save();
				DebugUtil.Log("OnClosing Completed", "udebug----------->");
			}
			catch (Exception e2)
			{
				DebugUtil.Log("OnClosing failed!", e2);
			}
		}

		private static void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				UmengAnalytics._manager.DoneAppError((Exception)e.ExceptionObject);
				UmengSettings.Save();
			}
			catch (Exception e2)
			{
				DebugUtil.Log(e2);
			}
		}

		private static void SetBodyFileName()
		{
			string text = AppInfo.GetVersion().Replace(".", "_");
			string text2 = UmengSettings.Get<string>("BodyFileName", null);
			if (!string.IsNullOrEmpty(text2) && text != text2)
			{
				BodyPersistentManager.Current.FileName = text2;
				BodyPersistentManager.Current.Delete();
			}
			BodyPersistentManager.Current.FileName = text;
			UmengSettings.Put("BodyFileName", text);
		}

		private static bool IsValidAppkey(string appkey)
		{
			if (string.IsNullOrEmpty(appkey))
			{
				DebugUtil.Log("apkey is null or empty", "udebug----------->");
				return false;
			}
			return true;
		}

		private static bool IsValidChannel(string[] channels)
		{
			return channels != null && channels.Length > 0 && !string.IsNullOrEmpty(channels[0]);
		}

		private static bool IsValidInput(object input)
		{
			try
			{
				if (input == null)
				{
					DebugUtil.Log("input Object is null", "udebug----------->");
					bool result = false;
					return result;
				}
				if (input is string && string.IsNullOrEmpty(input as string))
				{
					DebugUtil.Log("input string is null , empty or whitespace", "udebug----------->");
					bool result = false;
					return result;
				}
			}
			catch (Exception)
			{
				bool result = false;
				return result;
			}
			return true;
		}

		private static bool RegisterEvents()
		{
			bool result = false;
			try
			{
                System.AppDomain.CurrentDomain.UnhandledException += Application_UnhandledException;
                result = true;
  
            }
			catch (Exception e)
			{
				DebugUtil.Log("RegisterEvents failed", e);
				result = false;
			}
			return result;
		}
        //private static void Current_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        //	{
        //	}

        private static void OnCheckUpdateCompleted(out bool handled, Dictionary<string, object> dic)
		{
			try
			{
				handled = false;
				if (UmengAnalytics.CheckUpdateCompleted != null)
				{
					UpdateEventArgs updateEventArgs = new UpdateEventArgs(dic);
					UmengAnalytics.CheckUpdateCompleted(0, updateEventArgs);
					handled = updateEventArgs.Handled;
				}
			}
			catch (Exception e)
			{
				handled = false;
				DebugUtil.Log(e);
			}
		}

		private static void OnlineParamCallback(Dictionary<string, string> onlineParams)
		{
			try
			{
				if (UmengAnalytics.UpdateOnlineParamCompleted != null)
				{
					OnlineParamEventArgs e = new OnlineParamEventArgs(onlineParams);
					UmengAnalytics.UpdateOnlineParamCompleted(0, e);
				}
			}
			catch (Exception e2)
			{
				DebugUtil.Log(e2);
			}
		}
	}
}
