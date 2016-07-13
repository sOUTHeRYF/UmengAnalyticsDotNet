using System;
using UmengSDK.Model;

namespace UmengSDK.Business
{
	internal interface ITracker
	{
		Body DataBody
		{
			get;
			set;
		}

		void AddLaunchSession(Launch session);

		void AddTerminalSession(Terminate session);

		void AddErrorLog(Error error);

		void AddEventLog(Event e);

		void AddEKVLog(EKV e);
	}
}
