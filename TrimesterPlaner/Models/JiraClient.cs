using RestSharp;
using System.Collections.Immutable;
using TrimesterPlaner.Extensions;

namespace TrimesterPlaner.Models
{
    public class JiraClient
    {
        private string BaseUri { get; } = "https://confluence.ivu.de/jira/rest/api/2/";
        private string SearchUri { get; } = "https://confluence.ivu.de/jira/rest/api/2/search";

        public JiraClient()
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

        public async Task<IEnumerable<Ticket>?> LoadTickets(string jql)
        {
            var parameters = new
            {
                jql,
                fields = string.Join(",", new List<string>
                {
                    "issuelinks",
                    "summary",
                    "customfield_24003", // T-Shirt
                    "customfield_22082", // Planungsperiode
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

            Dictionary<string, string> issueToTrimesterTask = new(from issue in tickets.issues
                                                                  from issueLink in issue.fields.issuelinks
                                                                  where issueLink.type.name.Equals("Implements")
                                                                  where issueLink.outwardIssue.fields.issuetype.name.Equals("Trimester Task")
                                                                  select new KeyValuePair<string, string>(issue.key, issueLink.outwardIssue.key));

            Dictionary<string, string> trimesterTaskToPlanningPeriod = await GetPlanningPeriods([.. issueToTrimesterTask.Values]);

            PlanningPeriodHelper planningPeriodHelper = new(issueToTrimesterTask, trimesterTaskToPlanningPeriod);

            return from issue in tickets.issues
                   select new Ticket()
                   {
                       Key = issue.key,
                       Summary = issue.fields.summary,
                       PlanningPeriod = planningPeriodHelper.GetTrimesterTaskPlanningPeriod(issue.key),
                       Promised = planningPeriodHelper.IsPromised(issue.key, issue.fields.customfield_22082?.value),
                       Shirt = issue.fields.customfield_24003?.value.ToShirtSize(),
                       OriginalEstimate = ConvertToPT(issue.fields.timeoriginalestimate),
                       RemainingEstimate = ConvertToPT(issue.fields.timeestimate),
                       TimeSpent = ConvertToPT(issue.fields.timespent),
                   };
        }

        private class PlanningPeriodHelper(Dictionary<string, string> issueToTrimesterTask, Dictionary<string, string> trimesterTaskToPlanningPeriod)
        {
            internal string GetTrimesterTaskPlanningPeriod(string issueKey)
            {
                if (!issueToTrimesterTask.TryGetValue(issueKey, out string? trimesterTask))
                {
                    return "";
                }
                if (!trimesterTaskToPlanningPeriod.TryGetValue(trimesterTask, out string? trimesterTaskPlanningPeriod))
                {
                    return "";
                }
                return trimesterTaskPlanningPeriod;
            }

            internal bool IsPromised(string issueKey, string? issuePlanningPeriod)
            {
                string trimesterTaskPlanningPeriod = GetTrimesterTaskPlanningPeriod(issueKey);
                if (string.IsNullOrEmpty(trimesterTaskPlanningPeriod))
                {
                    return false;
                }
                return trimesterTaskPlanningPeriod.Equals(issuePlanningPeriod);
            }
        }

        private async Task<Dictionary<string, string>> GetPlanningPeriods(ImmutableSortedSet<string> trimesterTaskKeys)
        {
            if (trimesterTaskKeys.Count == 0)
            {
                return [];
            }

            var parameters = new
            {
                jql = $"key in ({string.Join(",", trimesterTaskKeys)})",
                fields = "customfield_22917",
                maxResults = 100
            };

            var trimesterTasks = await Client.GetAsync<TrimesterTaskList>(SearchUri, parameters);
            if (trimesterTasks is null)
            {
                return [];
            }

            return new(from trimesterTask in trimesterTasks.issues
                       select new KeyValuePair<string, string>(trimesterTask.key, trimesterTask.fields.customfield_22917));
        }

        private static double? ConvertToPT(int? time)
        {
            if (time is null)
            {
                return null;
            }

            return Math.Round(Convert.ToDouble(time) / 28800, 2);
        }

        private RestClient Client { get; }

#pragma warning disable IDE1006 // Naming Styles
        private record TicketList(Issue[] issues);
        private record Issue(string key, Fields fields);
        private record Fields(IssueLink[] issuelinks, string summary, Shirt customfield_24003, PlanningPeriod customfield_22082, int? timeoriginalestimate, int? timeestimate, int? timespent);
        private record Shirt(string value);
        private record IssueLink(IssueLinkType type, LinkedIssue inwardIssue, LinkedIssue outwardIssue);
        private record IssueLinkType(string name);
        private record LinkedIssue(string key, LinkedIssueFields fields);
        private record LinkedIssueFields(LinkedIssueType issuetype);
        private record LinkedIssueType(string name);
        private record PlanningPeriod(string value);

        private record TrimesterTaskList(TrimesterTask[] issues);
        private record TrimesterTask(string key, TrimesterTaskFields fields);
        private record TrimesterTaskFields(string customfield_22917);
#pragma warning restore IDE1006 // Naming Styles
    }
}