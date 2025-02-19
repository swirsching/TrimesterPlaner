﻿using RestSharp;
using System.Diagnostics;

namespace TrimesterPlaner.Models
{
    public class ConfluenceClient
    {
        private string BaseUri { get; } = "https://confluence.ivu.de/rest/api/";
        private string ContentUri { get; } = "https://confluence.ivu.de/rest/api/content/{0}";

        public ConfluenceClient()
        {
            Client = new(new RestClientOptions(BaseUri));
            var cat = Environment.GetEnvironmentVariable("CAT", EnvironmentVariableTarget.User);
            if (cat is not null)
            {
                Client.AddDefaultHeader("Authorization", $"Bearer {cat}");
                HasCAT = true;
            }
            else
            {
                HasCAT = false;
            }
        }

        private record PageMetadata(string Title, int Version);
        private async Task<PageMetadata?> GetPageMetadata(int pageID)
        {
            var url = string.Format(ContentUri, pageID);
            var response = await Client.GetAsync<PageMetadataResponse>(url);
            if (response?.title is null)
            {
                return null;
            }
            return new(response.title, response.version.number);
        }

        public async void UpdatePage(int pageID, string content)
        {
            var metadata = await GetPageMetadata(pageID) ?? throw new Exception("Couldn't find the specified page.");

            var parameters = new
            {
                id = pageID,
                type = "page",
                title = metadata.Title,
                version = new { number = metadata.Version + 1 },
                body = new
                {
                    storage = new
                    {
                        value = $"{{html}}{content}{{html}}",
                        representation = "wiki",
                    },
                },
            };

            var url = string.Format(ContentUri, pageID);
            var request = new RestRequest(url, Method.Put).AddBody(parameters);
            await Client.PutAsync(request);
        }

        private RestClient Client { get; }

        public bool HasCAT { get; }

#pragma warning disable IDE1006 // Naming Styles
        private record PageMetadataResponse(string title, Version version);
        private record Version(int number);
#pragma warning restore IDE1006 // Naming Styles
    }
}