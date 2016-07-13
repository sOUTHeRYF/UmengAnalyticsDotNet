using System;

namespace UmengSDK.Third.Gzip
{
	internal enum BlockState
	{
		NeedMore,
		BlockDone,
		FinishStarted,
		FinishDone
	}
}
