using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UmengSDK.Business
{
	//[DataContract]
	public class OnlineParam
	{
	//	[DataMember]
		public Dictionary<string, string> Params
		{
			get;
			set;
		}

	//	[DataMember]
		public string LastUpdateTime
		{
			get;
			set;
		}

		public OnlineParam()
		{
			this.Params = new Dictionary<string, string>();
		}
	}
}
