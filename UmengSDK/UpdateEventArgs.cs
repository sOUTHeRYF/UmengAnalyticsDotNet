using System;
using System.Collections.Generic;
using UmengSDK.Common;

namespace UmengSDK
{
	public class UpdateEventArgs : EventArgs
	{
		public bool Handled;

		public bool update;

		public string ProductID = string.Empty;

		public string VersionName = string.Empty;

		public string UpdateLog = string.Empty;

		public UpdateEventArgs(Dictionary<string, object> updateInfo)
		{
			try
			{
				if (updateInfo != null && updateInfo.ContainsKey("update") && "yes".Equals(updateInfo.get_Item("update").ToString().ToLower()))
				{
					this.update = true;
					this.ProductID = (updateInfo.ContainsKey("path") ? (updateInfo.get_Item("path") as string) : this.ProductID);
					this.VersionName = (updateInfo.ContainsKey("version") ? (updateInfo.get_Item("version") as string) : this.VersionName);
					this.UpdateLog = (updateInfo.ContainsKey("update_log") ? (updateInfo.get_Item("update_log") as string) : this.UpdateLog);
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in init UpdateEventArgs", e);
			}
		}
	}
}
