using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmutableSerializationPOC
{
    public class Compression
    {
        private static string Compress(byte[] bytes)
        {
            byte[] byteArray;

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
                {
                    //Compress
                    gzip.Write(bytes, 0, bytes.Length);
                    //Close, DO NOT FLUSH cause bytes will go missing...
                    gzip.Close();

                    byteArray = ms.ToArray();
                    return ByteArrayToString(byteArray);
                }
            }
        }

        public static string Compress(string value)
        {
            //byte[] byteArray = Encoding.UTF8.GetBytes(value);
            byte[] byteArray = StringToByteArray(value);

            return Compress(byteArray);
        }

        public static string DeCompress(string value)
        {
            byte[] byteArray = StringToByteArray(value);

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    byteArray = new byte[0];
                    byte[] buffer = new byte[1024];
                    int nRead;
                    while ((nRead = gzip.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        int length = byteArray.Length;
                        Array.Resize<byte>(ref byteArray, length + nRead);
                        Array.Copy(buffer, 0, byteArray, length, nRead);
                    }

                    return ByteArrayToString(byteArray);
                }
            }
        }

        public static string Compress(Stream value)
        {
            byte[] byteArray = null;

            using (MemoryStream ms = new MemoryStream())
            {
                value.CopyTo(ms);
                byteArray = ms.ToArray();
                return Compress(byteArray);
            }
        }

        public static byte[] StringToByteArray(string value)
        {
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }
            return byteArray;
        }

        public static string ByteArrayToString(byte[] byteArray)
        {
            // GZip header is not correct !!!
            //return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            ////char[] chars = new char[byteArray.Length / sizeof(char)];
            //char[] chars = new char[byteArray.Length];
            //System.Buffer.BlockCopy(byteArray, 0, chars, 0, byteArray.Length);
            //return new string(chars);
            
            StringBuilder decompressed = new StringBuilder(byteArray.Length);
            foreach (byte item in byteArray)
            {
                decompressed.Append((char)item);
            }

            return decompressed.ToString();
        }
    }
}
