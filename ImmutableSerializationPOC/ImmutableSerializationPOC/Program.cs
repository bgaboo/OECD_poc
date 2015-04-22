using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ImmutableSerializationPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            ImmutableClass test = new ImmutableClass("name", "immutable", "private setter");

            //string serializedXml = XmlSerialize<ImmutableClass>(test);
            //Console.WriteLine(string.Format("XML serialized object: {0}", serializedXml));
            //ImmutableClass deserializedXml = null;
            //deserializedXml = XmlDeserialize<ImmutableClass>(serializedXml);

            //string serializedDC = DataContractSerialize<ImmutableClass>(test);
            //Console.WriteLine(string.Format("DataContract serialized object: {0}", serializedDC));

            string serializedNDC = NetDataContractSerialize<ImmutableClass>(test);
            Console.WriteLine(string.Format("NetDataContract serialized object: {0}", serializedNDC));
            ImmutableClass deserializedNDC = null;
            deserializedNDC = NetDataContractDeserialize<ImmutableClass>(serializedNDC);

            Console.ReadLine();
        }
        
        public static string NetDataContractSerialize<T>(T value)
        {
            if (value == null)
                return null;

            try
            {
                NetDataContractSerializer serializer = new NetDataContractSerializer();
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, value);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new StreamReader(stream).ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static T NetDataContractDeserialize<T>(string serialized)
        {
            try
            {
                NetDataContractSerializer serializer = new NetDataContractSerializer();
                using (MemoryStream stream = new MemoryStream())
                {
                    using (TextWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(serialized);
                        writer.Flush();
                        stream.Position = 0;
                        return (T)serializer.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
        }

        public static string XmlSerialize<T>(T value)
        {
            if (value == null)
                return null;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(stringWriter))
                    {
                        xmlSerializer.Serialize(writer, value);
                        return stringWriter.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static T XmlDeserialize<T>(string serialized)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(new StringReader(serialized));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
        }
    }
}
