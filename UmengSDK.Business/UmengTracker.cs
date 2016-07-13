using System;
using UmengSDK.Model;

namespace UmengSDK.Business
{
	internal class UmengTracker : ITracker
	{
		private Body _dataBody;

		public Body DataBody
		{
			get
			{
				return this._dataBody;
			}
			set
			{
				this._dataBody = value;
			}
		}

		public UmengTracker()
		{
			this._dataBody = new Body();
		}

		public void AddLaunchSession(Launch session)
		{
			if (this._dataBody != null)
			{
				this._dataBody.addLaunchSession(session);
			}
		}

		public void AddTerminalSession(Terminate session)
		{
			if (this._dataBody != null)
			{
				this._dataBody.addTerminalSession(session);
			}
		}

		public void AddErrorLog(Error error)
		{
			if (this._dataBody != null)
			{
				this._dataBody.addErrorLog(error);
			}
		}

		public void AddEventLog(Event e)
		{
			if (this._dataBody != null)
			{
				this._dataBody.addEventLog(e);
			}
		}

		public void AddEKVLog(EKV e)
		{
			if (this._dataBody != null)
			{
				this._dataBody.addEKVLog(e);
			}
		}
	}
}
