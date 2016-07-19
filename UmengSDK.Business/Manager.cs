//using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using UmengSDK.Common;
using UmengSDK.Model;

namespace UmengSDK.Business
{
	internal class Manager
	{
		public static string AppKey = null;

		public static string Channel = "Marketplace";

		private static string _sessionId = null;

		private ITracker _messageTracker;

		private Body _localSendingBody;

		private DateTime _resumeTime;

		private ReportPolicy _report_policy = ReportPolicy.BATCH_AT_LAUNCH;

		private string _currentPage;

		private List<object> _pages = new List<object>();

		private Dictionary<string, object> _currentPages = new Dictionary<string, object>();

		private ITracker MessageTracker
		{
			get
			{
				if (this._messageTracker == null)
				{
					lock (this)
					{
						if (this._messageTracker == null)
						{
							this._messageTracker = new AutoCachingDecorator(new UmengTracker());
						}
					}
				}
				return this._messageTracker;
			}
		}

		public Manager()
		{
			OnlineConfigManager.Current.UpdateCompletedEvent += new OnlineConfigManager.UpdateCompletedHandler(this.OnUpdateConfigCompleted);
		}

		public void AddEvent(string eventId)
		{
			this.MessageTracker.AddEventLog(new Event(eventId));
		}

		public void AddEvent(string eventId, string label)
		{
			this.MessageTracker.AddEventLog(new Event(eventId, label));
		}

		public void AddEvent(string eventId, long duration)
		{
			this.MessageTracker.AddEventLog(new Event(eventId, duration));
		}

		public void AddEvent(string eventId, string label, long duration)
		{
			this.MessageTracker.AddEventLog(new Event(eventId, label, duration));
		}

		public void AddEvent(string eventId, Dictionary<string, string> kv)
		{
			this.MessageTracker.AddEKVLog(new EKV(eventId, kv));
		}

		public void AddEvent(string eventId, Dictionary<string, string> kv, long duration)
		{
			this.MessageTracker.AddEKVLog(new EKV(eventId, kv, duration));
		}

		public void AddError(string error)
		{
			this.MessageTracker.AddErrorLog(new Error(error));
		}

		public void AddError(Exception e, string errMsg)
		{
			this.MessageTracker.AddErrorLog(new Error(e, errMsg));
		}

		public void DoneAppError(Exception e)
		{
			if (!string.IsNullOrEmpty(this._currentPage))
			{
				this.AddPageViewEnd(this._currentPage);
			}
			this.MessageTracker.DataBody.addErrorLog(new Error(e, ""));
			this.OnPause();
		}

		public void AddPageViewStart(string pageName)
		{
			if (this._currentPages.ContainsKey(pageName))
			{
				this._currentPages.Remove(pageName);
			}
			this._currentPages.Add(pageName, DateTime.Now.Ticks);
			this._currentPage = pageName;
		}

		public void AddPageViewEnd(string pageName)
		{
			if (this._currentPages != null && this._currentPages.ContainsKey(pageName))
			{
				long num = (long)this._currentPages[pageName];
				long num2 = (long)Math.Ceiling((double)(DateTime.Now.Ticks - num) / 10000000.0);
				this._pages.Add(string.Format("[\"{0}\",{1}]", pageName, num2));
				this._currentPages.Remove(pageName);
			}
			this._currentPage = string.Empty;
		}

		public void OnResume()
		{
			this._resumeTime = DateTime.Now;
			this._pages.Clear();
			this._currentPages.Clear();
			if (OnlineConfigManager.Current.Policy == ReportPolicy.INTERVAL)
			{
				this.StartPeriodicReport(OnlineConfigManager.Current.Interval);
			}
			if (this.ShouldStartNewSession())
			{
				new Thread(()=>
				{
					Manager._sessionId = this.CreateSessionId();
					this.MessageTracker.DataBody.SessionId = Manager._sessionId;
					Launch launch = new Launch(Manager._sessionId);
					Terminate terminate = Terminate.footprint();
					if (OnlineConfigManager.Current.Policy == ReportPolicy.BATCH_AT_LAUNCH)
					{
						this.SendCacheMessage(launch, terminate);
						UmengSettings.Put("LastReportTime", DateTime.Now);
					}
					else
					{
						if (launch != null)
						{
							this.MessageTracker.AddLaunchSession(launch);
						}
						if (terminate != null)
						{
							this.MessageTracker.AddTerminalSession(terminate);
						}
					}
					OnlineConfigManager.Current.Update();
				}).Start();
				return;
			}
			Manager._sessionId = UmengSettings.Get<string>("_umeng_session_id", null);
			this.MessageTracker.DataBody.SessionId = Manager._sessionId;
		}

		public void OnPause()
		{
			BodyPersistentManager.Current.Save(this.MessageTracker.DataBody);
			this._messageTracker = null;
			DateTime now = DateTime.Now;
			UmengSettings.Put("_umeng_last_pause_time", now.ToString());
			UmengSettings.Put("_umeng_session_id", Manager._sessionId);
			UmengSettings.Acc("_umeng_duration", (int)now.Subtract(this._resumeTime).TotalSeconds);
			if (this._pages != null && this._pages.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (List<object>.Enumerator enumerator = this._pages.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = (string)enumerator.Current;
						stringBuilder.Append(text);
						stringBuilder.Append(';');
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					UmengSettings.Acc("_umeng_activities", stringBuilder.ToString());
				}
			}
			else
			{
				DebugUtil.Log("No page be tracked!You can track it by calling TrackPageStart and TrackPageEnd.", "udebug----------->");
			}
			DebugUtil.Log(string.Format("----------------------- pause time : {0} ---------------------", now), "udebug----------->");
		}

		private string CreateSessionId()
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Manager.AppKey);
				stringBuilder.Append(DateTime.Now.Ticks);
				stringBuilder.Append(Header.Instance().getDeviceID());
				return MD5Core.GetHashString(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			}
			catch (Exception e)
			{
				DebugUtil.Log("Fail to make session id :", e);
			}
			return null;
		}

		private bool ShouldReport()
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				DebugUtil.Log("Network is unavailable, maybe missing permission ID_CAP_NETWORKING", "udebug----------->");
				return false;
			}
			bool result = true;
			switch (this._report_policy)
			{
			case ReportPolicy.BATCH_AT_LAUNCH:
				return result;
			case ReportPolicy.DAILY:
			{
				DateTime dateTime = DateTime.Parse(UmengSettings.Get<string>(Constants.Key_Last_Report_Time, null));
				if (DateTime.Now.Subtract(dateTime).CompareTo(Constants.One_Day) < 0)
				{
					result = false;
					return result;
				}
				return result;
			}
			case ReportPolicy.WIFIONLY:
                    result = true;
                    return result;
			}
			result = true;
			return result;
		}

		private bool ShouldStartNewSession()
		{
			string text = UmengSettings.Get<string>("_umeng_last_pause_time", null);
			if (string.IsNullOrEmpty(text))
			{
				DebugUtil.Log("start new session !", "udebug----------->");
				return true;
			}
			if (DateTime.Now.Subtract(DateTime.Parse(text)).CompareTo(TimeSpan.FromSeconds((double)Constants.SessionInterval)) > 0)
			{
				DebugUtil.Log("start new session !", "udebug----------->");
				return true;
			}
			DebugUtil.Log("extends current session !", "udebug----------->");
			return false;
		}

		private void SendCacheMessage(Launch launch, Terminate terminate)
		{
			try
			{
				this._localSendingBody = BodyPersistentManager.Current.LocalBody;
				if (this._localSendingBody == null)
				{
					this._localSendingBody = new Body();
				}
				else
				{
					BodyPersistentManager.Current.Delete();
				}
				if (launch != null)
				{
					this._localSendingBody.addLaunchSession(launch);
				}
				if (terminate != null)
				{
					this._localSendingBody.addTerminalSession(terminate);
				}
				AutoResetEvent sendEvent = new AutoResetEvent(false);
				new NetTask(this._localSendingBody)
				{
					SendResponseCallback = delegate(string result)
					{
						this.OnSendCacheLogCallback(result);
						sendEvent.Set();
					}
				}.sendMessage();
				sendEvent.WaitOne(5000);
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in sendCacheMessage Manager", e);
			}
		}

		private void OnSendCacheLogCallback(string result)
		{
			try
			{
				if (result != null)
				{
					DebugUtil.Log(string.Concat(new object[]
					{
						"send local body success : ",
						result,
						" at ",
						DateTime.Now
					}), "udebug----------->");
				}
				else
				{
					BodyPersistentManager.Current.Save(this._localSendingBody);
					DebugUtil.Log("send local body failed : " + result, "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("OnSendCacheLogCallback failed", e);
			}
		}

		private void OnUpdateConfigCompleted(bool isUpdated, OnlineConfigManager.OnlineConfig config)
		{
			try
			{
				if (isUpdated && config != null && config.Policy == ReportPolicy.INTERVAL)
				{
					this.StartPeriodicReport(config.Interval);
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("OnUpdateConfigCompleted failed", e);
			}
		}

		public void StartPeriodicReport(uint interval)
		{
			if (this.MessageTracker is PeriodicReportProxy)
			{
				PeriodicReportProxy periodicReportProxy = this.MessageTracker as PeriodicReportProxy;
				periodicReportProxy.Interval = interval;
				periodicReportProxy.IsSendLocal = true;
			}
			else
			{
				this._messageTracker = new PeriodicReportProxy(this.MessageTracker, interval, true);
			}
			DebugUtil.Log(string.Concat(new object[]
			{
				"Start Periodic Report with Interval: ",
				interval,
				" at ",
				DateTime.Now
			}), "udebug----------->");
		}
	}
}
