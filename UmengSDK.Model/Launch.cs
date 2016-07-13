using System;

namespace UmengSDK.Model
{
	internal class Launch : Time
	{
		public Launch(string sessionId)
		{
			base.setSession(sessionId);
		}

		public Launch()
		{
		}
	}
}
