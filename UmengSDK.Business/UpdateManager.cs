using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using UmengSDK.Common;

namespace UmengSDK.Business
{
	internal class UpdateManager
	{
		public delegate void CheckCompletedHandler(out bool handled, Dictionary<string, object> dic);

		public UpdateManager.CheckCompletedHandler CheckCompletedEvent;

		public void CheckUpdate()
		{
			try
			{
				ThreadPool.QueueUserWorkItem(delegate(object s)
				{
					DebugUtil.Log("Checking Update ...", "udebug----------->");
					new NetTask(MessageType.CheckUpdate, null)
					{
						SendResponseCallback = new NetTask.ResponseCallback(this.OnUpdateCallback)
					}.sendMessage();
				});
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		private void OnUpdateCallback(string result)
		{
			if (result != null)
			{
				DebugUtil.Log(result, "udebug----------->");
				try
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)JSON.JsonDecode(result);
					bool flag = false;
					if (this.CheckCompletedEvent != null)
					{
						this.CheckCompletedEvent(out flag, dictionary);
					}
					if (!flag && dictionary.ContainsKey("update") && "yes".Equals(dictionary.get_Item("update").ToString().ToLower()))
					{
						string version = dictionary.ContainsKey("version") ? (dictionary.get_Item("version") as string) : string.Empty;
						string description = dictionary.ContainsKey("update_log") ? (dictionary.get_Item("update_log") as string) : string.Empty;
						string link = dictionary.ContainsKey("path") ? (dictionary.get_Item("path") as string) : string.Empty;
						this.ShowUpdateDialog(version, description, link);
					}
					return;
				}
				catch (Exception e)
				{
					DebugUtil.Log(e);
					return;
				}
			}
			DebugUtil.Log("Fail to get Update info ...", "udebug----------->");
		}

		private void ShowUpdateDialog(string version, string description, string link)
		{
            /*
			StringBuilder uinfo = new StringBuilder();
			uinfo.Append("Latest version:").Append(version).Append("\n").Append(description);
			Deployment.get_Current().get_Dispatcher().BeginInvoke(delegate
			{
				MessageBoxResult messageBoxResult = MessageBox.Show(uinfo.ToString(), "New version found", 1);
				if (messageBoxResult == 1)
				{
					this.OpenMarket(link);
				}
			});
            */
		}

		private void OpenMarket(string link)
		{
			if (string.IsNullOrEmpty(link))
			{
				return;
			}
			try
			{
				WebBrowserTask webBrowserTask = new WebBrowserTask();
				webBrowserTask.set_Uri(new Uri(link));
				webBrowserTask.Show();
			}
			catch (Exception e)
			{
				DebugUtil.Log("Fail to open url : " + link, e);
			}
		}
	}
}
