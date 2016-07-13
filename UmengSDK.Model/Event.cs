using System;
using UmengSDK.Common;

namespace UmengSDK.Model
{
	internal class Event : Time
	{
		private int MAX_LENGTH_64 = 64;

		private int MAX_LENGTH_256 = 256;

		private string KEY_TAG = "tag";

		private string KEY_LABEL = "label";

		private string KEY_ACC = "acc";

		private string KEY_DURATION = "du";

		public long Duration
		{
			get
			{
				if (base.get(this.KEY_DURATION) != null)
				{
					return (long)base.get(this.KEY_DURATION);
				}
				return 0L;
			}
			set
			{
				base.put(this.KEY_DURATION, value);
			}
		}

		public Event(string tag)
		{
			tag = tag.CheckInput(this.MAX_LENGTH_64);
			base.put(this.KEY_TAG, tag);
			base.put(this.KEY_ACC, 1);
		}

		public Event(string tag, string label)
		{
			tag = tag.CheckInput(this.MAX_LENGTH_64);
			label = label.CheckInput(this.MAX_LENGTH_256);
			base.put(this.KEY_TAG, tag);
			base.put(this.KEY_LABEL, label);
			base.put(this.KEY_ACC, 1);
		}

		public Event(string tag, string label, int acc)
		{
			tag = tag.CheckInput(this.MAX_LENGTH_64);
			label = label.CheckInput(this.MAX_LENGTH_256);
			base.put(this.KEY_TAG, tag);
			base.put(this.KEY_LABEL, label);
			base.put(this.KEY_ACC, acc);
		}

		public Event(string tag, long duration)
		{
			tag = tag.CheckInput(this.MAX_LENGTH_64);
			base.put(this.KEY_TAG, tag);
			base.put(this.KEY_DURATION, duration);
			base.put(this.KEY_ACC, 1);
		}

		public Event(string tag, string label, long duration)
		{
			tag = tag.CheckInput(this.MAX_LENGTH_64);
			label = label.CheckInput(this.MAX_LENGTH_256);
			base.put(this.KEY_TAG, tag);
			base.put(this.KEY_LABEL, label);
			base.put(this.KEY_DURATION, duration);
			base.put(this.KEY_ACC, 1);
		}

		public bool hasLabel()
		{
			return base.get(this.KEY_LABEL) != null;
		}
	}
}
