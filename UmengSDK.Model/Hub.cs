using System;
using System.Collections.Generic;

namespace UmengSDK.Model
{
	internal class Hub
	{
		private Dictionary<string, object> hub;

		public Hub()
		{
			this.hub = new Dictionary<string, object>();
		}

		public Hub(Dictionary<string, object> dic)
		{
			this.hub = dic;
		}

		public Dictionary<string, object> ToDictionary()
		{
			return this.hub;
		}

		public void put(string key, object value)
		{
			if (string.IsNullOrEmpty(key) || value == null)
			{
				return;
			}
			if (this.hub.ContainsKey(key))
			{
				this.hub[key] = value;
				return;
			}
			this.hub.Add(key, value);
		}

		public object get(string key)
		{
			if (this.hub.ContainsKey(key))
			{
				return this.hub[key];
			}
			return null;
		}
	}
}
