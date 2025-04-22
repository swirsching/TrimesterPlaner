using RestSharp;

namespace Utilities.Services
{
    public class BaseJiraClient
    {
        protected string BaseUri { get; } = "https://confluence.ivu.de/jira/rest/api/2/";
        protected string SearchUri { get; } = "https://confluence.ivu.de/jira/rest/api/2/search";
        protected RestClient Client { get; }

        public BaseJiraClient()
        {
            Client = new(new RestClientOptions(BaseUri));
            var jat = Environment.GetEnvironmentVariable("JAT", EnvironmentVariableTarget.User);
            if (jat is not null)
            {
                Client.AddDefaultHeader("Authorization", $"Bearer {jat}");
            }
            else
            {
                throw new Exception("Missing your jira access token. Please create a jira access token and save it in an environment variable called JAT in order to use the program.");
            }
        }
    }
}