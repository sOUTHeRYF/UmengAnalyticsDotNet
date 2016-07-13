using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Threading;
using UmengSDK.Common;
using UmengSDK.Model;

namespace UmengSDK.Business
{
	internal class PeriodicReportProxy : ITracker
	{
		private uint _interval = 20u;

		private bool _isSendLocal;

		private DateTime _lastReportTime = DateTime.MinValue;

		private bool _isReporting;

		private ITracker _tracker;

		private Body _sendingBody;

		public Body DataBody
		{
			get
			{
				if (this._tracker != null)
				{
					return this._tracker.DataBody;
				}
				return null;
			}
			set
			{
				if (this._tracker != null)
				{
					this._tracker.DataBody = value;
				}
			}
		}

		public uint Interval
		{
			get
			{
				return this._interval;
			}
			set
			{
				if (this._interval >= 10u && this._interval <= 86400u)
				{
					this._interval = value;
					return;
				}
				this._interval = 20u;
			}
		}

		public bool IsSendLocal
		{
			get
			{
				return this._isSendLocal;
			}
			set
			{
				this._isSendLocal = value;
			}
		}

		public PeriodicReportProxy(ITracker tracker, uint interval = 30u, bool isSendLocal = false)
		{
			this._tracker = tracker;
			this.Interval = interval;
			this.IsSendLocal = isSendLocal;
			if (UmengSettings.Contains("LastReportTime"))
			{
				this._lastReportTime = UmengSettings.Get<DateTime>("LastReportTime", default(DateTime));
				return;
			}
			this._lastReportTime = DateTime.MinValue;
		}

		public void AddLaunchSession(Launch session)
		{
			if (this._tracker != null)
			{
				this._tracker.AddLaunchSession(session);
				this.Report();
			}
		}

		public void AddTerminalSession(Terminate session)
		{
			if (this._tracker != null)
			{
				this._tracker.AddTerminalSession(session);
				this.Report();
			}
		}

		public void AddErrorLog(Error error)
		{
			if (this._tracker != null)
			{
				this._tracker.AddErrorLog(error);
				this.Report();
			}
		}

		public void AddEventLog(Event e)
		{
			if (this._tracker != null)
			{
				this._tracker.AddEventLog(e);
				this.Report();
			}
		}

		public void AddEKVLog(EKV e)
		{
			if (this._tracker != null)
			{
				this._tracker.AddEKVLog(e);
				this.Report();
			}
		}

		private bool CanReport()
		{
			bool isNetworkAvailable = DeviceNetworkInformation.get_IsNetworkAvailable();
			bool flag = (DateTime.get_Now() - this._lastReportTime).get_TotalSeconds() >= this._interval;
			return isNetworkAvailable && flag && !this._isReporting;
		}

		private void Report()
		{
			if (this.CanReport())
			{
				this._isReporting = true;
				ThreadPool.QueueUserWorkItem(delegate(object s)
				{
					try
					{
						DebugUtil.Log("Periodic Report at " + DateTime.get_Now(), "udebug----------->");
						if (this._tracker.DataBody.IsSessionReady)
						{
							if (this.IsSendLocal)
							{
								BodyPersistentManager.Current.Load(this._tracker.DataBody);
								BodyPersistentManager.Current.Delete();
							}
							this._sendingBody = this._tracker.DataBody;
							this._tracker.DataBody = new Body();
							this._tracker.DataBody.SessionId = this._sendingBody.SessionId;
							new NetTask(this._sendingBody)
							{
								SendResponseCallback = new NetTask.ResponseCallback(this.OnReportCompleted)
							}.sendMessage();
						}
						else
						{
							this._isReporting = false;
							DebugUtil.Log("Session is not Ready,Report next time!", "udebug----------->");
						}
					}
					catch (Exception e)
					{
						this._isReporting = false;
						DebugUtil.Log(e);
					}
				});
				return;
			}
			DebugUtil.Log("Check Periodic Report at " + DateTime.get_Now(), "udebug----------->");
		}

		private void OnReportCompleted(string response)
		{
			try
			{
				if (!string.IsNullOrEmpty(response))
				{
					this._sendingBody = null;
					this._lastReportTime = DateTime.get_Now();
					UmengSettings.Put("LastReportTime", this._lastReportTime);
					DebugUtil.Log("Periodic Report successed : " + this._lastReportTime, "udebug----------->");
				}
				else
				{
					BodyPersistentManager.Current.Save(this._sendingBody);
					DebugUtil.Log("Periodic Report failed : " + DateTime.get_Now(), "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("OnReportCompleted failed", e);
			}
			this._isReporting = false;
		}
	}
}
