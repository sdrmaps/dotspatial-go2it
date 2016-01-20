// serializable dictionary to XML string, which is in turn stored to user settings file

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace SDR.Configuration
{
    public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            // TODO: 
            return null;
        }

        public string ToXmlString()
        {
            using (TextWriter writer = new Utf8StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(writer))
                {
                    WriteXml(xmlWriter);
                    xmlWriter.Close();
                }
                var str = writer.ToString();
                writer.Close();
                return str;
            }
        }

        public void FromXmlString(string xmlString)
        {
            using (TextReader reader = new StringReader(xmlString))
            {
                using (XmlReader xmlReader = XmlReader.Create(reader))
                {
                    ReadXml(xmlReader);
                    xmlReader.Close();
                }
                reader.Close();
            }
        }

        public void ReadXml(XmlReader reader)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            reader.ReadStartElement("Dictionary");
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("KeyValuePair");

                reader.ReadStartElement("Key");
                var key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("Value");
                var value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            writer.WriteStartElement("Dictionary");
            foreach (var key in Keys)
            {
                writer.WriteStartElement("KeyValuePair");

                writer.WriteStartElement("Key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("Value");
                var value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
