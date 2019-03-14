using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinzDownloader
{
    internal class Program
    {
        private async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            const string ADDRESS_REFERENCES = "table-53331";
            const long FEATURE_PAGE_SIZE = 10000;

            RestClient client = new RestClient("https://data.linz.govt.nz");

            long numHits = 0;

            {
                RestRequest request = new RestRequest("services;key=2adb3cdbb2f94735bc8bc8ae362ac0cc/wfs");
                request.Method = Method.GET;
                request.AddParameter("service", "WFS");
                request.AddParameter("version", "2.0.0");
                request.AddParameter("request", "GetFeature");
                request.AddParameter("typeNames", ADDRESS_REFERENCES);
                request.AddParameter("resultType", "hits"); // gets count of features only
                IRestResponse response = client.Execute(request);
                var xml = XDocument.Parse(response.Content);
                numHits = long.Parse(xml.Root.Attribute("numberMatched").Value);
            }
            Console.WriteLine("Num hits: " + numHits);
            //numHits = 100000;

            Console.WriteLine("Num hits: " + numHits);

            var tasks = new List<Task>();

            for (long i = 0; i < numHits; i += FEATURE_PAGE_SIZE)
            {
                tasks.Add(FetchBatch(i, FEATURE_PAGE_SIZE, ADDRESS_REFERENCES, client));
            }

            await Task.WhenAll(tasks.ToArray());
            Console.WriteLine("END");
            Console.ReadLine();
        }

        private static async Task FetchBatch(long startIndex, long count, string typeName, RestClient client)
        {
            RestRequest request = new RestRequest("services;key=2adb3cdbb2f94735bc8bc8ae362ac0cc/wfs");
            request.Method = Method.GET;
            request.AddParameter("service", "WFS");
            request.AddParameter("version", "2.0.0");
            request.AddParameter("request", "GetFeature");
            request.AddParameter("count", count);
            request.AddParameter("startIndex", startIndex);
            request.AddParameter("typeNames", typeName);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            string filePath = String.Format(
                "d:\\temp\\{0}-{1:D9}-{2:D9}.xml",
                typeName,
                startIndex,
                startIndex + count);
            await System.IO.File.WriteAllTextAsync(filePath, response.Content);
        }
    }
}