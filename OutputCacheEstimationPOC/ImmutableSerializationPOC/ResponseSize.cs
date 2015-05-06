using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImmutableSerializationPOC
{
    public class ResponseSize
    {
        private const string urlPrefix = "http://dotstat.oecd.org/SDMX-JSON/";
        private const string urlPartData = "data";
        private const string urlPartMetadata = "metadata";

        private static CookieContainer CookieContainer = new CookieContainer();

        public static void DoTest()
        {
            List<string> dataToSerialize = new List<string>();
            List<string> metadataToSerialize = new List<string>();

            //dataToSerialize.Add("DP_LIVE/.ULC.TOT.PC_CHGPY.Q/OECD?json-lang=fr&dimensionAtObservation=allDimensions&startPeriod=2007-Q4&endPeriod=2014-Q4"); // OK
            //dataToSerialize.Add("DP_LIVE/.ULC.TOT.PC_CHGPY.Q/OECD?json-lang=en&dimensionAtObservation=allDimensions&startPeriod=2007-Q4&endPeriod=2014-Q4"); // OK
            //dataToSerialize.Add("DP_LIVE/.GDP.TOT.USD_CAP.A/OECD?json-lang=en&dimensionAtObservation=allDimensions&startPeriod=1980"); // OK
            // replaced with the next dataToSerialize.Add("DP_LIVE/.GDP/OECD?json-lang=en&dimensionAtObservation=allDimensions&detail=SeriesKeysOnly");
            //dataToSerialize.Add("DP_LIVE/....GDP/OECD?json-lang=en&dimensionAtObservation=allDimensions&detail=SeriesKeysOnly"); // OK
            //dataToSerialize.Add("DP_LIVE/.CPI.TOT.AGRWTH.M/OECD?json-lang=en&dimensionAtObservation=allDimensions&startPeriod=2011");  // OK
            //dataToSerialize.Add("DP_LIVE/.GNI.TOT.USD_CAP.A/OECD?json-lang=en&dimensionAtObservation=allDimensions&startPeriod=1996"); // OK
            //dataToSerialize.Add("DP_LIVE/.CPI.../OECD?json-lang=en&dimensionAtObservation=allDimensions&detail=SeriesKeysOnly");
            //dataToSerialize.Add("DP_LIVE/.TRADEGOODSERV.../OECD?json-lang=en&dimensionAtObservation=allDimensions&detail=SeriesKeysOnly");
            //dataToSerialize.Add("DP_LIVE/AUS+AUT+BEL+CAN+CHL+CZE+DNK+EST+FIN+FRA+DEU+GRC+HUN+ISL+IRL+ISR+ITA+JPN+KOR+LUX+MEX+NLD+NZL+NOR+POL+PRT+SVK+SVN+ESP+SWE+CHE+TUR+GBR+USA+BRA+IDN+IND+CHN+RUS+ZAF.GDP.TOT.USD_CAP.A/OECD?json-lang=en&dimensionAtObservation=allDimensions&startPeriod=2013&endPeriod=2013"); // OK
            //dataToSerialize.Add("DP_LIVE/.NEET.20_24.PC_AGE.A/OECD?json-lang=en&dimensionAtObservation=allDimensions&startPeriod=2005"); // OK
            //dataToSerialize.Add("DP_LIVE/.NUTRBALANCE.NITROGEN.KG_HA.A/OECD?json-lang=en&dimensionAtObservation=allDimensions&startPeriod=2000"); // OK
            //dataToSerialize.Add("DP_LIVE/.UNEMP.../OECD?json-lang=en&dimensionAtObservation=allDimensions&detail=SeriesKeysOnly");
            //dataToSerialize.Add("DP_LIVE/.HEALTHEXP.../OECD?json-lang=en&dimensionAtObservation=allDimensions&detail=SeriesKeysOnly");
            //dataToSerialize.Add("MEI_PRICES/CPALTT.AUS+AUT+BEL+CAN+CHL+CZE+DNK+EST+FIN+FRA+DEU+GRC+HUN+ISL+IRL+ISR+ITA+JPN+KOR+LUX+MEX+NLD+NZL+NOR+POL+PRT+SVK+SVN+ESP+SWE+CHE+TUR+GBR+USA+EA17+EU27+G7M+OEU+OTO+NMEC+BRA+CHN+IND+IDN+RUS+ZAF.IXOB.A+Q+M/OECD?startPeriod=2010&endPeriod=2013"); // OK
            //dataToSerialize.Add("MEI_PRICES/CPALTT.USA.IXOB.A+Q+M/all?startTime=1950-03"); // OK
            //dataToSerialize.Add("MEI_PRICES/CPALTT.USA.GY.M/all?startTime=1950-03"); // OK
            //dataToSerialize.Add("MEI_PRICES/CPALTT.AUT+BEL+CAN+CHL+CZE+DNK+EST+FIN+FRA+DEU+GRC+HUN+ISL+IRL+ISR+ITA+JPN+KOR+LUX+MEX+NLD+NOR+POL+PRT+SVK+SVN+ESP+SWE+CHE+TUR+GBR+USA+EA19+EU28+G-7+OECDE+OECD+NMEC+BRA+CHN+COL+IND+IDN+LVA+RUS+ZAF.GY.M/all?startTime=1950"); // OK
            //dataToSerialize.Add("MEI_PRICES/CPALTT.AUS+AUT+BEL+CAN+CHL+CZE+DNK+EST+FIN+FRA+DEU+GRC+HUN+ISL+IRL+ISR+ITA+JPN+KOR+LUX+MEX+NLD+NZL+NOR+POL+PRT+SVK+SVN+ESP+SWE+CHE+TUR+GBR+USA+EA19+EU28+G-7+OECDE+OECD+NMEC+BRA+CHN+COL+IND+IDN+LVA+RUS+ZAF.IXOB.A+Q+M/all?startTime=1950"); // OK
            //dataToSerialize.Add("DP_STAGING/AUT.TRADEGOODSERV.NTRADE.BLN_USD.M/OECD?json-lang=en&dimensionAtObservation=allDimensions"); // OK
            //dataToSerialize.Add("DP_STAGING/GBR.GRANT.TOT.MLN_USD.A/OECD?json-lang=en&dimensionAtObservation=allDimensions"); // OK
            //dataToSerialize.Add("MEI_FIN/IRSTCI.USA.M/all?startTime=1950-03"); // OK
            //dataToSerialize.Add("MEI_FIN/IRSTCI.AUS+AUT+BEL+CAN+CHL+CZE+DNK+EST+FIN+FRA+DEU+GRC+HUN+ISL+IRL+ISR+ITA+JPN+KOR+LUX+MEX+NLD+NZL+NOR+POL+PRT+SVK+SVN+ESP+SWE+CHE+TUR+GBR+USA+EA19+SDR+NMEC+BRA+CHN+COL+IND+IDN+LVA+RUS+ZAF.M/all?startTime=1950-03&endTime=2015-03"); // OK
            //dataToSerialize.Add("QNA/all/all?startTime=2009-Q1&endTime=2011-Q4"); // OK
            //dataToSerialize.Add("QNA/AUS+AUT.GDP+B1_GE.CUR+VOBARSA.Q/all?startTime=2009-Q2&endTime=2011-Q4"); // OK
            //dataToSerialize.Add("REFSERIES/AUS+CAN/OECD?startPeriod=2005");  // unauthorized
            dataToSerialize.Add("KEI");
            //dataToSerialize.Add("KEI/LRHUTTTT.AUS.M/all");

            //metadataToSerialize.Add("CSPCUBE/all"); // OK

            Dictionary<string, Tuple<long, long, long>> dataResultsSerialized = new Dictionary<string, Tuple<long, long, long>>();
            Dictionary<string, Tuple<long, long, long>> metadataResultsSerialized = new Dictionary<string, Tuple<long, long, long>>();

            Dictionary<string, string> m = new Dictionary<string, string>();

            foreach (var item in dataToSerialize)
            {
                try
                {
                    Stream result = GetStream(CreateRequest(item, true));
                    string resultString = StreamToString(result);
                    // DEVNOTE: ConnectStream cannot seek and can only be read once, therfore should be retrieved again
                    result = GetStream(CreateRequest(item, true));
                    Stopwatch sw = Stopwatch.StartNew();
                    string compressedResult = Compression.Compress(result);
                    sw.Stop();
                    // DEVNOTE: for checking the correctness of the Compres Decompress
                    string decompressedResult = Compression.DeCompress(compressedResult);

                    if (resultString != decompressedResult)
                    {
                        //throw new Exception("The decompressed and the original value differs!");
                        Console.WriteLine("{0}\tThe decompressed and the original value differs!", item);
                    }

                    dataResultsSerialized.Add(item,
                        new Tuple<long, long, long>(resultString.Length, compressedResult.Length, sw.ElapsedTicks));
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception with: {0}", item);
                    Console.WriteLine("Exception: {0}", ex.Message);
                }
            }

            foreach (var item in metadataToSerialize)
            {
                try
                {
                    Stream result = GetStream(CreateRequest(item, false));
                    string resultString = StreamToString(result);
                    // DEVNOTE: ConnectStream cannot seek and can only be read once, therfore should be retrieved again
                    result = GetStream(CreateRequest(item, false));
                    Stopwatch sw = Stopwatch.StartNew();
                    string compressedResult = Compression.Compress(result);
                    sw.Stop();
                    // DEVNOTE: for checking the correctness of the Compres Decompress
                    string decompressedResult = Compression.DeCompress(compressedResult);

                    if (resultString != decompressedResult)
                    {
                        //throw new Exception("The decompressed and the original value differs!");
                        Console.WriteLine("{0}\tThe decompressed and the original value differs!", item);
                    }

                    metadataResultsSerialized.Add(item,
                        new Tuple<long, long, long>(resultString.Length, compressedResult.Length, sw.ElapsedTicks));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception with: {0}", item);
                    Console.WriteLine("Exception: {0}", ex.Message);
                }
            }

            Visualize(dataResultsSerialized);
            Visualize(metadataResultsSerialized);

            Console.WriteLine();
            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static void Visualize(Dictionary<string, Tuple<long, long, long>> results)
        {
            foreach (var item in results)
            {
                //Console.WriteLine("{0}\t{1}\t{2}\t{3:f2}%\t{4}", item.Key.Substring(0, item.Key.Length < 15 ? item.Key.Length : 15), item.Value.Item1, item.Value.Item2, 1.0 - (item.Value.Item2 / Convert.ToDouble(item.Value.Item1)), item.Value.Item3);
                Console.WriteLine("{0}\t{1}\t{2}\t{3:f2}%\t{4}", item.Key, item.Value.Item1, item.Value.Item2, 1.0 - (item.Value.Item2 / Convert.ToDouble(item.Value.Item1)), item.Value.Item3);
            }
        }

        private static Stream GetStream(HttpWebRequest request)
        {
            request.Timeout = 30000000;
            request.Method = "Get";
            request.CookieContainer = CookieContainer;

            Stream responseStream = null;

            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                responseStream = httpWebResponse.GetResponseStream();
            }
            catch (WebException webException)
            {
                if (webException.Response != null)
                {
                    using (HttpWebResponse httpWebResponse = (HttpWebResponse)webException.Response)
                    {
                        if (HttpStatusCode.OK != httpWebResponse.StatusCode)
                        {
                            string msg = string.Format("The response status is not the expected. Expected: {0}, got {1}", HttpStatusCode.OK, httpWebResponse.StatusCode);
                            throw new Exception(msg);
                            //Console.WriteLine("Exception with: {0}", request.Address.AbsoluteUri);
                            //Console.WriteLine("Exception: {0}", msg);
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Format("Unexpected exception occured {0}", webException));
                }
            }

            return responseStream;
        }

        private static HttpWebRequest CreateRequest(string query, bool isDataRequest)
        {
            Uri url = new Uri(string.Format("{0}{1}/{2}", urlPrefix, isDataRequest ? urlPartData : urlPartMetadata, query));
            //return (HttpWebRequest)WebRequest.Create(url);
            CookieContainer myContainer = new CookieContainer();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = GetCredential(url);
            request.CookieContainer = myContainer;
            request.PreAuthenticate = false;
            return request;
        }

        private static CredentialCache GetCredential(Uri uri)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(uri, "Basic", new NetworkCredential("username", "pw"));
            return credentialCache;
        }

        private static string StreamToString(Stream value)
        {
            byte[] byteArray = null;

            using (MemoryStream ms = new MemoryStream())
            {
                value.CopyTo(ms);
                byteArray = ms.ToArray();
                return Compression.ByteArrayToString(byteArray);
            }
        }
    }
}
