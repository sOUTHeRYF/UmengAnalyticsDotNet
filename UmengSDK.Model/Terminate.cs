using System;
using System.Collections.Generic;
using UmengSDK.Common;

namespace UmengSDK.Model
{
	internal class Terminate : Time
	{
		private string KEY_DURATION = "duration";

		private string KEY_ACTIVITY = "activities";

		public Terminate()
		{
			base.resetTime(DateTime.Parse(UmengSettings.Get<string>("_umeng_last_pause_time", null)));
			try
			{
				base.put(this.KEY_DURATION, UmengSettings.Get<int>("_umeng_duration", 0));
				base.put(this.KEY_ACTIVITY, this.getPagePath());
			}
			catch (Exception e)
			{
				DebugUtil.Log("fail to parce duration & page info", e);
			}
		}

		public static Terminate footprint()
		{
			string text = UmengSettings.Get<string>("_umeng_session_id", null);
			if (text == null)
			{
				return null;
			}
			Terminate terminate = null;
			try
			{
				terminate = new Terminate();
				terminate.setSession(text);
				terminate.clearFootprint();
			}
			catch (Exception e)
			{
				DebugUtil.Log("fail to last session data", e);
			}
			return terminate;
		}

		private List<object> getPagePath()
		{
			string text = UmengSettings.Get<string>("_umeng_activities", null);
			List<object> list = new List<object>();
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					';'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						List<object> list2 = null;
						try
						{
							list2 = (List<object>)JSON.JsonDecode(array[i]);
						}
						catch
						{
						}
						if (list2 != null)
						{
							list.Add(list2);
						}
					}
				}
			}
			return list;
		}

		private void clearFootprint()
		{
			UmengSettings.Clear("_umeng_activities");
			UmengSettings.Clear("_umeng_duration");
			UmengSettings.Clear("_umeng_session_id");
		}
	}
}
