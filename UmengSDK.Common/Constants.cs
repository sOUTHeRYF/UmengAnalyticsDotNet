using System;

namespace UmengSDK.Common
{
	internal class Constants
	{
		internal const string KEY_SESSION_ID = "_umeng_session_id";

		internal const string KEY_DURATION = "_umeng_duration";

		internal const string KEY_ACTIVITIES = "_umeng_activities";

		internal const string KEY_LAST_PAUSE_TIME = "_umeng_last_pause_time";

		internal const string KEY_LAST_REPORT_TIME = "LastReportTime";

		internal static readonly string SDK_TYPE = "wphone";

		internal static readonly string SDK_TYPE_CONFIG = "wp";

		internal static readonly string SDK_VERSION = "2.0.1";

		internal static readonly string OS = "WINDOWSPHONE OS 8.0";

		internal static readonly string[] SEND_LOG_URL = new string[]
		{
			"http://www.umeng.com/app_logs",
			"http://www.umeng.co/app_logs"
		};

		internal static readonly string[] SEND_UPDATE_URL = new string[]
		{
			"http://www.umeng.com/api/check_app_update",
			"http://www.umeng.co/api/check_app_update"
		};

		internal static readonly string[] SEND_CONFIG_URL = new string[]
		{
			"http://www.umeng.com/check_config_update",
			"http://www.umeng.co/check_config_update"
		};

		internal static readonly string SEND_SUCESS = "{\"success\":\"ok\"}";

		internal static bool locationEnabled = true;

		internal static bool get_user_info = false;

		internal static long check_update_delay = 1000L;

		internal static long check_config_delay = 2000L;

		internal static long send_log_delay = 0L;

		internal static readonly bool Flag_Locking = true;

		internal static DateTime UTC = new DateTime(1970, 1, 1, 0, 0, 0, 1);

		internal static TimeSpan One_Day = new TimeSpan(24, 0, 0);

		private static string KEY_SESSIONINTERVAL = "SessionInterval";

		internal static bool mergeLog = false;

		public static string fake_log = "";

		internal static string Key_Age = "_umeng_age";

		internal static string Key_Gender = "_umeng_gender";

		internal static string Key_UserId = "_umeng_id";

		internal static string Key_Source = "_umeng_source";

		internal static string Key_Last_Config_Time = "_umeng_last_report_time";

		internal static string Key_Report_policy = "_umeng_report_policy";

		internal static string Key_Last_Report_Time = "_umeng_last_report_time";

		internal static long SessionInterval
		{
			get
			{
				long result;
				if (UmengSettings.Contains(Constants.KEY_SESSIONINTERVAL))
				{
					result = UmengSettings.Get<long>(Constants.KEY_SESSIONINTERVAL, 0L);
				}
				else
				{
					result = 30L;
				}
				return result;
			}
			set
			{
				UmengSettings.Put(Constants.KEY_SESSIONINTERVAL, value);
			}
		}

		internal static bool IsDebug
		{
			get;
			set;
		}
	}
}
