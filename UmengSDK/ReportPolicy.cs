using System;

namespace UmengSDK
{
	public enum ReportPolicy
	{
		REALTIME,
		BATCH_AT_LAUNCH,
		BATCH_AT_TERMINATE,
		PUSH,
		DAILY,
		WIFIONLY,
		INTERVAL
	}
}
