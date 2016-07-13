using System;
using System.Threading;
using UmengSDK.Common;
using UmengSDK.Model;

namespace UmengSDK.Business
{
	internal class AutoCachingDecorator : ITracker
	{
		private int MAX_COUNT = 5120;

		private ITracker _tracker;

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
				lock (this)
				{
					if (this._tracker != null)
					{
						this._tracker.DataBody = value;
					}
				}
			}
		}

		public AutoCachingDecorator(ITracker tracker)
		{
			this._tracker = tracker;
		}

		public void AddLaunchSession(Launch session)
		{
			try
			{
				lock (this)
				{
					if (this._tracker != null)
					{
						this._tracker.AddLaunchSession(session);
						this.Check();
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public void AddTerminalSession(Terminate session)
		{
			try
			{
				lock (this)
				{
					if (this._tracker != null)
					{
						this._tracker.AddTerminalSession(session);
						this.Check();
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public void AddErrorLog(Error error)
		{
			try
			{
				lock (this)
				{
					if (this._tracker != null)
					{
						this._tracker.AddErrorLog(error);
						this.Check();
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		public void AddEventLog(Event e)
		{
			try
			{
				lock (this)
				{
					if (this._tracker != null)
					{
						this._tracker.AddEventLog(e);
						this.Check();
					}
				}
			}
			catch (Exception e2)
			{
				DebugUtil.Log(e2);
			}
		}

		public void AddEKVLog(EKV e)
		{
			try
			{
				lock (this)
				{
					if (this._tracker != null)
					{
						this._tracker.AddEKVLog(e);
						this.Check();
					}
				}
			}
			catch (Exception e2)
			{
				DebugUtil.Log(e2);
			}
		}

		private void Check()
		{
			int bodySize = this._tracker.DataBody.Size;
			if (bodySize >= this.MAX_COUNT)
			{
				Body oldBody = this._tracker.DataBody;
				this._tracker.DataBody = new Body();
				ThreadPool.QueueUserWorkItem(delegate(object s)
				{
					if (BodyPersistentManager.Current.Save(oldBody))
					{
						DebugUtil.Log("auto save body successed on count: " + bodySize, "udebug----------->");
						return;
					}
					this._tracker.DataBody.Merge(oldBody);
				});
			}
			DebugUtil.Log("body's count: " + bodySize, "udebug----------->");
		}
	}
}
