using System;
using UmengClassLibrary.UmengSDK.Model;

namespace UmengSDK.Model
{
	internal class Time : Hub, ISessionIdentity
	{
		public string KEY_DATE = "date";

		public string KEY_TIME = "time";

		public string KEY_SESSION_ID = "session_id";

		public Time()
		{
			base.put(this.KEY_DATE, DateTime.Now.ToString("yyyy-MM-dd"));
			base.put(this.KEY_TIME, DateTime.Now.ToString("HH:mm:ss"));
		}

		public void setSession(string sessionId)
		{
			base.put(this.KEY_SESSION_ID, sessionId);
		}

		public void resetTime(DateTime newTime)
		{
			base.put(this.KEY_DATE, newTime.ToString("yyyy-MM-dd"));
			base.put(this.KEY_TIME, newTime.ToString("HH:mm:ss"));
		}
	}
}
