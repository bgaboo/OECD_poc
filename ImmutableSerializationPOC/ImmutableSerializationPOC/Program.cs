using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            string keyNDC = "NetDataContract";
            string keyNJ = "NewtonSoftJson";
            string keyP = "ProtoBuff";

            ImmutableClass test = new ImmutableClass("name", "immutable", "private setter");
            ImmutableClassSS test2 = new ImmutableClassSS("name", "immutable", "private setter");
            ImmutableClassProto test3 = new ImmutableClassProto("name", "immutable", "private setter");

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

            Console.WriteLine();

            string serializedNJ = NewtonSoftJsonSerialize<ImmutableClass>(test);
            Console.WriteLine(string.Format("NewtonSoft JSON serialized object: {0}", serializedNJ));
            ImmutableClass deserializedNJ = null;
            deserializedNJ = NewtonSoftJsonDeserialize<ImmutableClass>(serializedNJ);

            Console.WriteLine();

            // DEVNOTE: DataMember attribuites on private fields did not work for ServiceStack JSON serializer
            // it also does not work with properties without setter and does not work with private members

            string serializedSJ = ServiceStackJsonSerialize<ImmutableClassSS>(test2);
            Console.WriteLine(string.Format("ServiceStack JSON serialized object: {0}", serializedSJ));
            ImmutableClassSS deserializedSJ = null;
            deserializedSJ = ServiceStackJsonDeserialize<ImmutableClassSS>(serializedSJ);

            Console.WriteLine();

            string serializedP = ProtoSerialize<ImmutableClassProto>(test3);
            Console.WriteLine(string.Format("ProtoBuf serialized object: {0}", serializedP));
            ImmutableClassProto deserializedP = null;
            deserializedP = ProtoDeserialize<ImmutableClassProto>(serializedP);

            Console.WriteLine();
            Console.WriteLine();

            Dictionary<string, List<long>> resultsSerialize = new Dictionary<string, List<long>>();
            resultsSerialize.Add(keyNDC, new List<long>());
            resultsSerialize.Add(keyNJ, new List<long>());
            resultsSerialize.Add(keyP, new List<long>());

            Dictionary<string, List<long>> resultsDeserialize = new Dictionary<string, List<long>>();
            resultsDeserialize.Add(keyNDC, new List<long>());
            resultsDeserialize.Add(keyNJ, new List<long>());
            resultsDeserialize.Add(keyP, new List<long>());

            Stopwatch sw;
            for (int i = 0; i < 1000; i++)
            {
                // NetDataContract
                sw = Stopwatch.StartNew();
                serializedNDC = NetDataContractSerialize<ImmutableClass>(test);
                sw.Stop();
                resultsSerialize[keyNDC].Add(sw.ElapsedTicks);

                sw = Stopwatch.StartNew();
                deserializedNDC = NetDataContractDeserialize<ImmutableClass>(serializedNDC);
                sw.Stop();
                resultsDeserialize[keyNDC].Add(sw.ElapsedTicks);

                // NewtonSoftJson
                sw = Stopwatch.StartNew();
                serializedNJ = NewtonSoftJsonSerialize<ImmutableClass>(test);
                sw.Stop();
                resultsSerialize[keyNJ].Add(sw.ElapsedTicks);

                sw = Stopwatch.StartNew();
                deserializedNJ = NewtonSoftJsonDeserialize<ImmutableClass>(serializedNJ);
                sw.Stop();
                resultsDeserialize[keyNJ].Add(sw.ElapsedTicks);

                // ProtoBuf
                sw = Stopwatch.StartNew();
                serializedP = ProtoSerialize<ImmutableClassProto>(test3);
                sw.Stop();
                resultsSerialize[keyP].Add(sw.ElapsedTicks);

                sw = Stopwatch.StartNew();
                deserializedP = ProtoDeserialize<ImmutableClassProto>(serializedP);
                sw.Stop();
                resultsDeserialize[keyP].Add(sw.ElapsedTicks);
            }

            foreach (var item in resultsSerialize)
            {
                Console.WriteLine(string.Format("{0} serialization AVG {1} ticks", item.Key, item.Value.Average()));
            }
            foreach (var item in resultsDeserialize)
            {
                Console.WriteLine(string.Format("{0} deserialization AVG {1} ticks", item.Key, item.Value.Average()));
            }

            Console.ReadLine();
        }

        public static string ProtoSerialize<T>(T value)
        {
            if (value == null)
                return null;

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, value);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new StreamReader(stream).ReadToEnd();
                }
                // XmlWriter
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static T ProtoDeserialize<T>(string serialized)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (TextWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(serialized);
                        writer.Flush();
                        stream.Position = 0;
                        return ProtoBuf.Serializer.Deserialize<T>(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
        }

        public static string ServiceStackJsonSerialize<T>(T value)
        {
            if (value == null)
                return null;

            try
            {
                return ServiceStack.Text.JsonSerializer.SerializeToString<T>(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static T ServiceStackJsonDeserialize<T>(string serialized)
        {
            try
            {
                return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(serialized);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
        }

        public static string NewtonSoftJsonSerialize<T>(T value)
        {
            if (value == null)
                return null;

            try
            {
                JsonSerializer serializer = JsonSerializer.Create();
                using (TextWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, value);
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static T NewtonSoftJsonDeserialize<T>(string serialized)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(serialized);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
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
