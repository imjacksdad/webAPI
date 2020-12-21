

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    throw new Exception("Error: job number and application arguments are Required.");
                }
                string API_KEY =  ConfigurationManager.AppSettings.Get("API_KEY");
              string usdaUrl =  ConfigurationManager.AppSettings.Get("usdaUrl");
              string filePath = ConfigurationManager.AppSettings.Get("filePath");
                HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(usdaUrl + args[0] + "&marketYear=" + args[1] );
                GETRequest.Method = "GET";
                GETRequest.Accept = "application/json";
                GETRequest.Headers.Add("API_KEY", API_KEY);

                Console.WriteLine("Sending GET Request");
                HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
                Stream GETResponseStream = GETResponse.GetResponseStream();
                StreamReader sr = new StreamReader(GETResponseStream);

                Console.WriteLine("Response from Server");
                //   Console.WriteLine(sr.ReadToEnd());
                var t = JsonToCsv3(sr.ReadToEnd().ToString(), ",");
                File.WriteAllText(filePath + "WorldCommodityData" + args[0] + "-" + args[1] + ".csv", t.ToString());
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        private static object JsonToCsv3(string jsonContent, string delimiter)
        {

            var data = jsonStringToTable(jsonContent);
            var headers = ((IEnumerable<dynamic>)((IEnumerable<dynamic>)data).First()).Select((prop) => prop.Name).ToArray();
            var csvList = new List<string>
        {
            string.Join(delimiter, headers.Select((prop) => string.Format(@"""{0}""", prop)).ToArray())
        };

            var lines = ((IEnumerable<dynamic>)data)
                .Select(row => row)
                .Cast<IEnumerable<dynamic>>()
                .Select((instance) => string.Join(delimiter, instance.Select((v) => string.Format(@"""{0}""", v.Value))))
                .ToArray();

            csvList.AddRange(lines);
            return string.Join(Environment.NewLine, csvList);
        }
        static private dynamic jsonStringToTable(string jsonContent)
        {
            var json = jsonContent.Split(new[] { '=' }).Last();
            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        static private IEnumerable<T> jsonStringToTable<T>(string jsonContent) where T : class
        {
            var json = jsonContent.Split(new[] { '=' }).Last();
            return JsonConvert.DeserializeObject<IEnumerable<T>>(json);
        }
    }


}
