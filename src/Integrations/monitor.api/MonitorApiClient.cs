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
            string content = "";
            HttpClient client = new HttpClient();

            client.BaseAddress = new System.Uri(baseApiUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string pingUrl = "ping?code=" + System.Environment.GetEnvironmentVariable("DIGI_API_CODE_PING");

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, pingUrl);
            req.Content = new StringContent(content, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = client.GetAsync(pingUrl).Result)
            {
                using (HttpContent cont = response.Content)
                {
                    var json = cont.ReadAsStringAsync().Result;
                }
            }
        }

        // TODO: Trekk ut felleskomponenter til egne metoder
        //  Task<int> .. var tidl: async
        public static int FindOrCreateEnhetOppsett(EnhetOppsett enhetOppsett)
        {
            string findOrCreateEnhetUrl = "find-or-create-enhet-oppsett?code=" +  System.Environment.GetEnvironmentVariable("DIGI_API_CODE_ENHET");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseApiUrl + findOrCreateEnhetUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(enhetOppsett);
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

        public static  int FindOrCreateFunksjonellTest(String metodenavnTest, String funksjoneltNavnTest)
        {
            string findOrCreateFunksjonellTestUrl = "find-or-create-funksjonell-test?code="  + System.Environment.GetEnvironmentVariable("DIGI_API_CODE_FUNKTEST");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseApiUrl + findOrCreateFunksjonellTestUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{'metodenavnTest': '" + metodenavnTest + "', 'funksjoneltNavnTest': '" + funksjoneltNavnTest + "'}";
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

        public static  int PostTestkjoring(Testkjoring testkjoring)
        {
            string postTestKjoringUrl = "post-testkjoring?code=" + System.Environment.GetEnvironmentVariable("DIGI_API_CODE_TESTKJORING");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseApiUrl + postTestKjoringUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(testkjoring);

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