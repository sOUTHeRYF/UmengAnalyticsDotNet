using System;
using System.Collections.Generic;
using UmengSDK.Common;

namespace UmengSDK.Model
{
	internal class EKV : Hub
	{
		public const string NumberKey = "__ct__";

		private int MAX_LENGTH_64 = 64;

		private string KEY_EVENT_ID = "id";

		private string KEY_TIMESTAMP = "ts";

		private string KEY_DURATION = "du";

		private Dictionary<string, object> kv;

		public EKV(string id, Dictionary<string, string> kv)
		{
			this.init(id, kv);
		}

		public EKV(string id, Dictionary<string, string> kv, long duration)
		{
			base.put(this.KEY_DURATION, duration);
			this.init(id, kv);
		}

		private void init(string id, Dictionary<string, string> kv)
		{
			id = id.CheckInput(this.MAX_LENGTH_64);
			base.put(this.KEY_EVENT_ID, id);
			base.put(this.KEY_TIMESTAMP, (long)DateTime.Now.Subtract(Constants.UTC).TotalSeconds);
			int num = 0;
			using (Dictionary<string, string>.Enumerator enumerator = kv.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.Current;
					num++;
					if (num > 10)
					{
						break;
					}
					string text = current.Key;
					text = text.CheckInput(this.MAX_LENGTH_64);
					string text2 = current.Value;
					int num2 = 0;
					if (current.Key == "__ct__" && int.TryParse(current.Value, out num2))
					{
						base.put(text, num2);
					}
					else
					{
						text2 = text2.CheckInput(256);
						base.put(text, text2);
					}
				}
			}
		}

		public EKV(Dictionary<string, object> dic)
		{
			if (dic.ContainsKey(this.KEY_EVENT_ID))
			{
				base.put(this.KEY_EVENT_ID, dic[this.KEY_EVENT_ID] as string);
				dic.Remove(this.KEY_EVENT_ID);
			}
			if (dic.ContainsKey(this.KEY_DURATION))
			{
				base.put(this.KEY_DURATION, (long)dic[this.KEY_DURATION]);
				dic.Remove(this.KEY_DURATION);
			}
			if (dic.ContainsKey(this.KEY_TIMESTAMP))
			{
				base.put(this.KEY_TIMESTAMP, (long)dic[this.KEY_TIMESTAMP]);
				dic.Remove(this.KEY_TIMESTAMP);
			}
			if (dic.Count > 0)
			{
				this.kv = dic;
			}
		}
	}
}
