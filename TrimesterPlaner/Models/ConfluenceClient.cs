using RestSharp;

namespace TrimesterPlaner.Models
{
    public class ConfluenceClient
    {
        private string BaseUri { get; } = "https://confluence.ivu.de/jira/rest/api/2/";

        public ConfluenceClient()
        {
            Client = new(new RestClientOptions(BaseUri));
            var cat = Environment.GetEnvironmentVariable("CAT", EnvironmentVariableTarget.User);
            if (cat is not null)
            {
                Client.AddDefaultHeader("Authorization", $"Bearer {cat}");
            }
        }

        public void UpdatePage(string content)
        {
            throw new NotImplementedException();
        }

        private RestClient Client { get; }
    }
}