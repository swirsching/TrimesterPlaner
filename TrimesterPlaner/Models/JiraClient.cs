﻿using RestSharp;
using System.Collections.Immutable;
using TrimesterPlaner.Extensions;

namespace TrimesterPlaner.Models
{
    public class JiraClient
    {
        public JiraClient()
        {
            Client = new(new RestClientOptions("https://confluence.ivu.de/jira/rest/api/2/"));
            Client.AddDefaultHeader("Authorization", "Bearer OTg5MjMyMDY5Mzk0Olk3tmB8BUoScs5a0g5eMKuK9yfG");
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
            var tickets = await Client.GetAsync<TicketList>("https://confluence.ivu.de/jira/rest/api/2/search", parameters);
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
                       Promised = planningPeriodHelper.IsPromised(issue.key, issue.fields.customfield_22082.value),
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

            internal bool IsPromised(string issueKey, string issuePlanningPeriod)
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
            var parameters = new
            {
                jql = $"key in ({string.Join(",", trimesterTaskKeys)})",
                fields = "customfield_22917",
                maxResults = 100
            };

            var trimesterTasks = await Client.GetAsync<TrimesterTaskList>("https://confluence.ivu.de/jira/rest/api/2/search", parameters);
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