using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Threading;
using UmengSDK.Common;

namespace UmengSDK.Business
{
	public class OnlineConfigManager
	{
		public delegate void UpdateCompletedHandler(bool isUpdated, OnlineConfigManager.OnlineConfig config);

	//	[DataContract]
		public class OnlineConfig
		{
			private uint _interval = 30u;

		//	[DataMember]
			public ReportPolicy Policy
			{
				get;
				set;
			}

		//	[DataMember]
			public string LastConfigTime
			{
				get;
				set;
			}

		//	[DataMember]
			public uint Interval
			{
				get
				{
					return this._interval;
				}
				set
				{
					this._interval = value;
				}
			}

			public OnlineConfig()
			{
				this.Policy = ReportPolicy.BATCH_AT_LAUNCH;
				this.Interval = 30u;
			}
		}

		private const string KEY_CONFIG = "config";

		private const string local_rp = "umeng_report_policy";

		private const string local_lct = "umeng_last_config_time";

		private const string KEY_CONFIG_TIME = "last_config_time";

		private const string KEY_CONFIG_POLICY = "report_policy";

		private const string KEY_CONFIG_PARAMS = "online_params";

		public const string KEY_REPORT_INTERVAL = "report_interval";

        private Dictionary<string, object> _isoSettings = new Dictionary<string, object>();

		private static readonly object synObj = new object();

		private static OnlineConfigManager _current = null;

		private bool _isUpdating;

		private OnlineConfigManager.OnlineConfig _config;

		public event OnlineConfigManager.UpdateCompletedHandler UpdateCompletedEvent;

		public static OnlineConfigManager Current
		{
			get
			{
				if (OnlineConfigManager._current == null)
				{
					lock (OnlineConfigManager.synObj)
					{
						if (OnlineConfigManager._current == null)
						{
							OnlineConfigManager._current = new OnlineConfigManager();
						}
					}
				}
				return OnlineConfigManager._current;
			}
		}

		public ReportPolicy Policy
		{
			get
			{
				return this._config.Policy;
			}
		}

		public uint Interval
		{
			get
			{
				return this._config.Interval;
			}
		}

		public string LastConfigTime
		{
			get
			{
				return this._config.LastConfigTime;
			}
		}

		private OnlineConfigManager()
		{
			if (!this.LoadFile())
			{
				this._config = new OnlineConfigManager.OnlineConfig();
			}
		}

		public void Update()
		{
			try
			{
				if (!this._isUpdating)
				{
					ThreadPool.QueueUserWorkItem(delegate(object s)
					{
						try
						{
							DebugUtil.Log("Updating online Config...", "udebug----------->");
							this._isUpdating = true;
							new NetTask(MessageType.OnlineConfig, null)
							{
								SendResponseCallback = new NetTask.ResponseCallback(this.OnConfigCallback)
							}.sendMessage();
						}
						catch (Exception e2)
						{
							DebugUtil.Log(e2);
						}
					});
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
			}
		}

		private void OnConfigCallback(string result)
		{
			try
			{
				if (result != null)
				{
					bool isUpdated = false;
					Dictionary<string, object> dictionary = (Dictionary<string, object>)JSON.JsonDecode(result);
					if (dictionary.ContainsKey("config_update") && "no".Equals(dictionary["config_update"].ToString().ToLower()))
					{
						DebugUtil.Log("has no online config update", "udebug----------->");
					}
					else
					{
						lock (this)
						{
							isUpdated = true;
							if (dictionary.ContainsKey("report_policy"))
							{
								this._config.Policy = (ReportPolicy)((long)dictionary["report_policy"]);
							}
							if (dictionary.ContainsKey("last_config_time"))
							{
								this._config.LastConfigTime = (dictionary["last_config_time"] as string);
							}
							if (dictionary.ContainsKey("report_interval"))
							{
								long num = (long)dictionary["report_interval"];
								this._config.Interval = (uint)num;
							}
							this.SaveFile();
						}
						DebugUtil.Log("Update Online Config Successed", "udebug----------->");
						DebugUtil.Log("OnlineConfig: " + result, "udebug----------->");
					}
					if (this.UpdateCompletedEvent != null)
					{
						this.UpdateCompletedEvent(isUpdated, this._config);
					}
				}
				else
				{
					DebugUtil.Log("Fail to get online Config ...", "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("Fail to get online Config ...", e);
			}
			finally
			{
				this._isUpdating = false;
			}
		}

		private void SaveFile()
		{
			try
			{
				lock (this)
				{
                    if (null != this._config)
                    {
                        IsoPersistentHelper.Save<OnlineConfig>(this._config, "umeng_olconfig");
                    }     
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in Save config", e);
			}
		}

		private bool LoadFile()
		{
			bool result = false;
			try
			{
				lock (this)
				{
                    OnlineConfig resultObj = IsoPersistentHelper.Load<OnlineConfig>("umeng_olconfig");
                    this._config = resultObj;
                }
			}
			catch (Exception e)
			{
				DebugUtil.Log("load online config from local failed!", e);
				result = false;
			}
			return result;
		}
	}
}
