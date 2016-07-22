using System;
using System.Collections.Generic;
using System.Linq;

namespace UmengSDK.Model
{
	internal class Body : Hub
	{
		private string KEY_LAUNCH = "launch";

		private string KEY_TERMINATE = "terminate";

		private string KEY_EVENT = "event";

		private string KEY_ERROR = "error";

		private string KEY_EKV = "ekv";

		public List<object> launches;

		public List<object> terminates;

		public List<object> eventLogs;

		public List<object> errorLogs;

		public List<object> ekvlogs;

		private List<object> ekvBuffer;

		public string SessionId
		{
			get;
			set;
		}

		public bool IsSessionReady
		{
			get
			{
				return !string.IsNullOrEmpty(this.SessionId);
			}
		}

		public int Size
		{
			get
			{
				int num = 0;
				if (this.launches != null)
				{
					num += this.launches.Count;
				}
				if (this.terminates != null)
				{
					num += this.terminates.Count;
				}
				if (this.eventLogs != null)
				{
					num += this.eventLogs.Count;
				}
				if (this.errorLogs != null)
				{
					num += this.errorLogs.Count;
				}
				if (this.ekvlogs != null)
				{
					num += this.ekvlogs.Count;
				}
				if (this.ekvBuffer != null)
				{
					num += this.ekvBuffer.Count;
				}
				return num;
			}
		}

		public Body()
		{
		}

		public Body(Dictionary<string, object> dic)
		{
			if (dic == null)
			{
				return;
			}
			if (dic.ContainsKey(this.KEY_ERROR))
			{
				this.errorLogs = (dic[this.KEY_ERROR] as List<object>);
			}
			if (dic.ContainsKey(this.KEY_EVENT))
			{
				this.eventLogs = (dic[this.KEY_EVENT] as List<object>);
			}
			if (dic.ContainsKey(this.KEY_LAUNCH))
			{
				this.launches = (dic[this.KEY_LAUNCH] as List<object>);
			}
			if (dic.ContainsKey(this.KEY_TERMINATE))
			{
				this.terminates = (dic[this.KEY_TERMINATE] as List<object>);
			}
			if (dic.ContainsKey(this.KEY_EKV))
			{
				this.ekvlogs = (dic[this.KEY_EKV] as List<object>);
			}
		}

		public void addLaunchSession(Launch session)
		{
			lock (this)
			{
				if (this.launches == null)
				{
					this.launches = new List<object>();
				}
				this.launches.Add(session.ToDictionary());
			}
		}

		public void addTerminalSession(Terminate session)
		{
			lock (this)
			{
				if (this.terminates == null)
				{
					this.terminates = new List<object>();
				}
				this.terminates.Add(session.ToDictionary());
			}
		}

		public void addErrorLog(Error error)
		{
			lock (this)
			{
				if (this.errorLogs == null)
				{
					this.errorLogs = new List<object>();
				}
				this.errorLogs.Add(error.ToDictionary());
			}
		}

		public void addEventLog(Event e)
		{
			lock (this)
			{
				if (this.eventLogs == null)
				{
					this.eventLogs = new List<object>();
				}
				this.eventLogs.Add(e.ToDictionary());
			}
		}

		public void addEKVLog(EKV e)
		{
			lock (this)
			{
				if (this.ekvBuffer == null)
				{
					this.ekvBuffer = new List<object>();
				}
				this.ekvBuffer.Add(e.ToDictionary());
			}
		}

		public void Merge(Body newBody)
		{
			if (newBody != null)
			{
				this.subMerge(newBody.errorLogs, ref this.errorLogs);
				this.subMerge(newBody.eventLogs, ref this.eventLogs);
				this.subMerge(newBody.ekvlogs, ref this.ekvlogs);
				this.subMerge(newBody.launches, ref this.launches);
				this.subMerge(newBody.terminates, ref this.terminates);
			}
		}

		private void subMerge(List<object> src, ref List<object> des)
		{
			lock (this)
			{
				if (des == null)
				{
					des = src;
				}
				else if (src != null && src.Count > 0)
				{
					des.AddRange(src);
				}
			}
		}

		public new Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> result;
			lock (this)
			{
				this.setSessionId();
				if (this.errorLogs != null)
				{
					base.put(this.KEY_ERROR, this.errorLogs);
				}
				if (this.eventLogs != null)
				{
					base.put(this.KEY_EVENT, this.eventLogs);
				}
				if (this.launches != null)
				{
					base.put(this.KEY_LAUNCH, this.launches);
				}
				if (this.terminates != null)
				{
					base.put(this.KEY_TERMINATE, this.terminates);
				}
				if (this.ekvlogs != null)
				{
					base.put(this.KEY_EKV, this.ekvlogs);
				}
				result = base.ToDictionary();
			}
			return result;
		}

		public void setSessionId()
		{
			lock (this)
			{
				if (this.IsSessionReady)
				{
					this.setSessionId(this.eventLogs, this.SessionId);
					if (this.ekvBuffer != null && this.ekvBuffer.Count > 0)
					{
						if (this.ekvlogs == null)
						{
							this.ekvlogs = new List<object>();
						}
						if (this.ekvlogs.Count > 0)
						{
							Dictionary<string, object> dictionary = this.ekvlogs[this.ekvlogs.Count - 1] as Dictionary<string, object>;
							if (dictionary != null && dictionary.Count > 0 && dictionary.ContainsKey(this.SessionId))
							{
								List<object> list = dictionary[this.SessionId] as List<object>;
								list.AddRange(this.ekvBuffer);
								this.ekvBuffer = new List<object>();
								return;
							}
						}
						List<object> arg_E9_0 = this.ekvlogs;
						Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
						dictionary2.Add(this.SessionId, this.ekvBuffer);
						arg_E9_0.Add(dictionary2);
						this.ekvBuffer = new List<object>();
					}
				}
			}
		}

		private void setSessionId(List<object> list, string sessionId)
		{
			lock (this)
			{
				if (list != null && list.Count > 0)
				{
					using (IEnumerator<object> enumerator = Enumerable.Where<object>(list, (object t) => !(t as Dictionary<string, object>).ContainsKey("session_id")).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object current = enumerator.Current;
							(current as Dictionary<string, object>).Add("session_id", sessionId);
						}
					}
				}
			}
		}

		public void Clear()
		{
			lock (this)
			{
				if (this.launches != null)
				{
					this.launches.Clear();
				}
				if (this.terminates != null)
				{
					this.terminates.Clear();
				}
				if (this.eventLogs != null)
				{
					this.eventLogs.Clear();
				}
				if (this.errorLogs != null)
				{
					this.errorLogs.Clear();
				}
				if (this.ekvlogs != null)
				{
					this.ekvlogs.Clear();
				}
				if (this.ekvBuffer != null)
				{
					this.ekvBuffer.Clear();
				}
			}
		}
	}
}
