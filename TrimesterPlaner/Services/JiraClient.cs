using RestSharp;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Models;
using Utilities.Services;

namespace TrimesterPlaner.Services
{
    public interface IJiraClient
    {
        public Task<IEnumerable<Ticket>?> LoadTickets(string jql, bool isInJQL);
    }

    public class JiraClient : BaseJiraClient, IJiraClient
    {
        public async Task<IEnumerable<Ticket>?> LoadTickets(string jql, bool isInJQL)
        {
            var parameters = new
            {
                jql,
                fields = string.Join(",", new List<string>
                {
                    "issuelinks",
                    "summary",
                    "customfield_24003", // T-Shirt
                    "customfield_22486", // Rank
                    "timeoriginalestimate",
                    "timeestimate",
                    "timespent",
                }),
                maxResults = 500
            };
            var tickets = await Client.GetAsync<TicketList>(SearchUri, parameters);
            if (tickets is null)
            {
                return null;
            }

            return from issue in tickets.issues
                   select new Ticket()
                   {
                       Key = issue.key,
                       Summary = issue.fields.summary,
                       Shirt = issue.fields.customfield_24003?.value.ToShirtSize(),
                       Rank = issue.fields.customfield_22486,
                       OriginalEstimate = ConvertToPT(issue.fields.timeoriginalestimate),
                       RemainingEstimate = ConvertToPT(issue.fields.timeestimate),
                       TimeSpent = ConvertToPT(issue.fields.timespent),
                       IsInJQL = isInJQL,
                   };
        }

        private static double? ConvertToPT(int? time)
        {
            if (time is null)
            {
                return null;
            }

            return Math.Round(Convert.ToDouble(time) / 28800, 2);
        }

#pragma warning disable IDE1006 // Naming Styles
        private record TicketList(Issue[] issues);
        private record Issue(string key, Fields fields);
        private record Fields(IssueLink[] issuelinks, string summary, Shirt customfield_24003, string customfield_22486, int? timeoriginalestimate, int? timeestimate, int? timespent);
        private record Shirt(string value);
        private record IssueLink(IssueLinkType type, LinkedIssue inwardIssue, LinkedIssue outwardIssue);
        private record IssueLinkType(string name);
        private record LinkedIssue(string key, LinkedIssueFields fields);
        private record LinkedIssueFields(LinkedIssueType issuetype);
        private record LinkedIssueType(string name);
#pragma warning restore IDE1006 // Naming Styles
    }
}