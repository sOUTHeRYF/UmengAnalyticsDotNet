using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Threading;
using UmengSDK.Common;

namespace UmengSDK.Business
{
	internal class OnlineParamManager
	{
		public delegate void UpdateCompletedHandler(Dictionary<string, string> onlineParams);

		private const string KEY_CACHE_PARAMS = "OnlineParams";

		private const string KEY_PARAMS = "online_params";

		private static IsolatedStorageSettings _isoSettings = IsolatedStorageSettings.get_ApplicationSettings();

		public OnlineParamManager.UpdateCompletedHandler UpdateCompletedEvent;

		private OnlineParam _onlineParam;

		public OnlineParamManager()
		{
			if (!this.LoadFile())
			{
				this._onlineParam = new OnlineParam();
			}
		}

		public void Update()
		{
			try
			{
				ThreadPool.QueueUserWorkItem(delegate(object s)
				{
					try
					{
						DebugUtil.Log("Updating online Params...", "udebug----------->");
						new NetTask(MessageType.OnlineParam, MessageBuilder.buildParamMessage(this._onlineParam.LastUpdateTime))
						{
							SendResponseCallback = new NetTask.ResponseCallback(this.OnParamsCallback)
						}.sendMessage();
					}
					catch (Exception e2)
					{
						DebugUtil.Log(e2);
					}
				});
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		private void OnParamsCallback(string result)
		{
			try
			{
				if (result != null)
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)JSON.JsonDecode(result);
					if (dictionary.ContainsKey("config_update") && "no".Equals(dictionary.get_Item("config_update").ToString().ToLower()))
					{
						DebugUtil.Log("has no online params update", "udebug----------->");
					}
					else
					{
						if (dictionary.ContainsKey("online_params"))
						{
							try
							{
								DebugUtil.Log(JSON.JsonEncode(dictionary.get_Item("online_params") as Dictionary<string, object>), "udebug----------->");
							}
							catch
							{
							}
							lock (this)
							{
								this._onlineParam.Params.Clear();
								Dictionary<string, object> dictionary2 = dictionary.get_Item("online_params") as Dictionary<string, object>;
								using (Dictionary<string, object>.Enumerator enumerator = dictionary2.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										KeyValuePair<string, object> current = enumerator.get_Current();
										this._onlineParam.Params.Add(current.get_Key(), current.get_Value() as string);
									}
								}
								this.SaveFile();
								DebugUtil.Log("Update Online Params Successed", "udebug----------->");
							}
						}
						if (this.UpdateCompletedEvent != null)
						{
							this.UpdateCompletedEvent(this._onlineParam.Params);
						}
					}
				}
				else
				{
					DebugUtil.Log("Fail to get online params ...", "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("Fail to get online params ...", e);
			}
		}

		private void SaveFile()
		{
			try
			{
				lock (this)
				{
					if (OnlineParamManager._isoSettings.Contains("OnlineParams"))
					{
						OnlineParamManager._isoSettings.set_Item("OnlineParams", this._onlineParam);
					}
					else
					{
						OnlineParamManager._isoSettings.Add("OnlineParams", this._onlineParam);
					}
					OnlineParamManager._isoSettings.Save();
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in Save online params", e);
			}
		}

		private bool LoadFile()
		{
			bool result;
			try
			{
				lock (this)
				{
					if (OnlineParamManager._isoSettings.Contains("OnlineParams"))
					{
						this._onlineParam = (OnlineParamManager._isoSettings.get_Item("OnlineParams") as OnlineParam);
					}
					result = (this._onlineParam != null);
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("load online params from local failed!", e);
				result = false;
			}
			return result;
		}

		public string GetParam(string key)
		{
			try
			{
				if (this._onlineParam.Params != null && this._onlineParam.Params.ContainsKey(key))
				{
					return this._onlineParam.Params.get_Item(key);
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("get online param failed with key: !" + key, e);
			}
			return string.Empty;
		}
	}
}
