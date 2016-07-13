using System;
using System.Collections.Generic;

namespace UmengSDK
{
	public class OnlineParamEventArgs : EventArgs
	{
		public bool IsUpdate;

		public Dictionary<string, string> Result;

		public OnlineParamEventArgs(Dictionary<string, string> onlineParams)
		{
			if (onlineParams != null && onlineParams.get_Count() > 0)
			{
				this.IsUpdate = true;
				this.Result = onlineParams;
			}
		}
	}
}
