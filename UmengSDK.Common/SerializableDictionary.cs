using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace UmengSDK.Common
{
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		public void WriteXml(XmlWriter write)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValue));
			for (int i = 0; i < base.get_Count(); i++)
			{
				write.WriteStartElement("SerializableDictionary");
				write.WriteStartElement("key");
				xmlSerializer.Serialize(write, Enumerable.ElementAt<KeyValuePair<TKey, TValue>>(this, i).get_Key());
				write.WriteEndElement();
				write.WriteStartElement("value");
				xmlSerializer2.Serialize(write, Enumerable.ElementAt<KeyValuePair<TKey, TValue>>(this, i).get_Value());
				write.WriteEndElement();
				write.WriteEndElement();
			}
		}

		public void ReadXml(XmlReader reader)
		{
			reader.Read();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValue));
			while (reader.get_NodeType() != 15)
			{
				reader.ReadStartElement("SerializableDictionary");
				reader.ReadStartElement("key");
				TKey tKey = (TKey)((object)xmlSerializer.Deserialize(reader));
				reader.ReadEndElement();
				reader.ReadStartElement("value");
				TValue tValue = (TValue)((object)xmlSerializer2.Deserialize(reader));
				reader.ReadEndElement();
				reader.ReadEndElement();
				base.Add(tKey, tValue);
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		public XmlSchema GetSchema()
		{
			return null;
		}
	}
}
