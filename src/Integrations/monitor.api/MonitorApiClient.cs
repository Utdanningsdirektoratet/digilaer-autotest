using System;
using System.Text;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using monitor.api.dto;

namespace monitor.api
{
    public class MonitorApiClient
    {
        private static string baseApiUrl = "https://digiliaermonitorapiaf.azurewebsites.net/api/";

        public static async void PingApi()
        {
            Console.WriteLine("Wake the Azure-function from sleep");
            string content = "";
            HttpClient client = new HttpClient();

            client.BaseAddress = new System.Uri(baseApiUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string pingUrl = "ping?code=SLFribEKLIAG1ZvSrARu0b81ug2beT0wtREcfiwMY1RPjnRcj1YBiw==";

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, pingUrl);
            req.Content = new StringContent(content, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = client.GetAsync(pingUrl).Result)
            {
                Console.WriteLine("Resp: " + response);
                Console.WriteLine("Resp2: " + response.Content);
                Console.WriteLine("Resp3: " + response.StatusCode);
                Console.WriteLine("" + response.ToString());

                using (HttpContent cont = response.Content)
                {
                    var json = cont.ReadAsStringAsync().Result;
                    Console.WriteLine("Json: " + json);
                }
            }
        }

        // TODO: Trekk ut felleskomponenter til egne metoder
        //  Task<int> .. var tidl: async
        public static int FindOrCreateEnhetOppsett(EnhetOppsett enhetOppsett)
        {
            string findOrCreateEnhetUrl = "find-or-create-enhet-oppsett?code=YReLjNgzTCfOkQOQJAFY0R0tYmWnLCEOZ8VjXv/Bvhgv3NY9NyACxQ==";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseApiUrl + findOrCreateEnhetUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(enhetOppsett);
                Console.WriteLine("Json: " + json);
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result = "-1";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result =  streamReader.ReadToEnd();
            }
            //return  new OkObjectResult(result);
            return Int32.Parse(result);
        }

        public static  int FindOrCreateFunksjonellTest(String metodenavnTest, String funksjoneltNavnTest)
        {
            string findOrCreateFunksjonellTestUrl = "find-or-create-funksjonell-test?code=pre8ud1bUaCL0Z28R5tBESwWKOewvaUXwz8ro3pTCd1Be2Zt863jFw==";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseApiUrl + findOrCreateFunksjonellTestUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{'metodenavnTest': '" + metodenavnTest + "', 'funksjoneltNavnTest': '" + funksjoneltNavnTest + "'}";
                Console.WriteLine("Json: " + json);
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result = "-1";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result =  streamReader.ReadToEnd();
            }
            // return  new OkObjectResult(result);
            return Int32.Parse(result);
        }

        public static  int PostTestkjoring(Testkjoring testkjoring)
        {
            string postTestKjoringUrl = "post-testkjoring?code=b/APCErgokFnPdOQkiYoWAgHALF6mAgzaXjPQKn3apXPCJl7fal67w==";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseApiUrl + postTestKjoringUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(testkjoring);

                Console.WriteLine(json);
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result = "-1";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result =  streamReader.ReadToEnd();
            }
            return Int32.Parse(result);
        }
    }
}