using System;
using System.Text;
using UmengSDK.Common;

namespace UmengSDK.Model
{
	internal class Error : Time
	{
		private string KEY_CONTEXT = "context";

		public Error(string error)
		{
			error = error.CheckInput(256);
			base.put(this.KEY_CONTEXT, error);
		}

		public Error(Exception e, string errMsg = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(errMsg))
			{
				stringBuilder.AppendLine(errMsg.CheckInput(256));
			}
			if (e != null)
			{
				stringBuilder.AppendLine(e.Message);
				stringBuilder.AppendLine(e.StackTrace);
			}
			base.put(this.KEY_CONTEXT, (stringBuilder.Length <= 0) ? null : stringBuilder.ToString());
		}
	}
}
