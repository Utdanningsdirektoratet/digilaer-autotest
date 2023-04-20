using System.Net.Http;
using System.Text;
using Utils;

namespace Slack
{
    public class SlackClient 
    {
        private static string baseSlackUrl = "https://hooks.slack.com/";
        private static string slackHookUrl = "services/" + System.Environment.GetEnvironmentVariable("DIGI_SLACK_WEBHOOK_PW");
        private static string slackUserName = "Digilær Auto-";

        public static void CallSlack(string slackText)
        {
            HttpClient client = new HttpClient();
            
            string slackEmoji =  ":digilaer:"; 
            string slackChannel = "#autotest_digilær_" + (GlobalVariables.ErTest() ? "stage" : GlobalVariables.Miljo);

            string content = "{\"channel\" : \"" + slackChannel + "\", \"username\": \"" + slackUserName + GlobalVariables.Miljo + "\", \"text\": \""+ slackText +"\", \"icon_emoji\": \"" + slackEmoji + "\"}";
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