using System;
using System.Net;
using System.Threading;

namespace UmengSDK.Business
{
	internal class HttpWebRequestState
	{
		public HttpWebRequest Request;

		public AutoResetEvent TimeoutEvent;

		public HttpWebRequestState(HttpWebRequest request)
		{
			this.Request = request;
			this.TimeoutEvent = new AutoResetEvent(false);
		}
	}
}
