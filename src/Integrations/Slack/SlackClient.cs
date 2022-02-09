using System.Net.Http;
using System.Text;

namespace Slack
{
    public class SlackClient 
    {
        private static string baseSlackUrl = "https://hooks.slack.com/";
        private static string slackHookUrl = "services/" + System.Environment.GetEnvironmentVariable("DIGI_SLACK_WEBHOOK_PW");
        private static string slackChannel = "#autotest_digilær";

        private static string slackUserName = "Digilær Autotest";

        public static async void CallSlack(string slackText)
        {
            HttpClient client = new HttpClient();
            
            string slackEmoji =  ":digilaer:";

            string content = "{\"channel\" : \"" + slackChannel + "\", \"username\": \"" + slackUserName + "\", \"text\": \""+ slackText +"\", \"icon_emoji\": \"" + slackEmoji + "\"}";
            client.BaseAddress = new System.Uri(baseSlackUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, slackHookUrl);
            req.Content = new StringContent(content, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = client.PostAsync(slackHookUrl, req.Content).Result)
            {
                using (HttpContent cont = response.Content)
                {
                    var json = cont.ReadAsStringAsync().Result;
                }
            }
        }
    }
}