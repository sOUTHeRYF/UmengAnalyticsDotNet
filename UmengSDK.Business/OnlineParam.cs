using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UmengSDK.Common;
namespace UmengSDK.Business
{
	//[DataContract]
	public class OnlineParam
	{
        //	[DataMember]
        public SerializableDictionary<string, string> Params;
 //       public Dictionary<string, string> Params;
        //	[DataMember]
        public string LastUpdateTime;

		public OnlineParam()
		{
			this.Params = new SerializableDictionary<string, string>();
            this.LastUpdateTime = string.Empty;
		}
	}
}
