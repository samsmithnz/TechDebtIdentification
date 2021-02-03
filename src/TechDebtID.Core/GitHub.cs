using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TechDebtID.Core.Statistics;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TechDebtID.Core
{
    public class GitHub
    {

        //A lot of steps here.
        //1. Connect to GitHub and get a list of repos
        //2. Log into Azure Storage and clone each repo to blogs
        //3. Run our scanner on the repos/blogs


        public async Task<List<string>> GetGitHubRepos(string organization)
        {
            List<string> repos = new List<string>();

            JsonElement resultJson = await GetGitHubReposFromAPI("", "", organization);
            foreach (JsonElement repoJson in resultJson.EnumerateArray())
            {
                JsonElement jsonElement = new JsonElement();
                if (repoJson.TryGetProperty("full_name", out jsonElement) == true)
                {
                    repos.Add(jsonElement.ToString());
                }
            }
            return repos;
        }

        public async Task<JsonElement> GetGitHubReposFromAPI(string clientId, string clientSecret, string owner)
        {
            JsonElement result = new JsonElement();
            //https://docs.github.com/en/rest/reference/repos
            //GET /repos/:owner/:repo/pulls/:pull_number/commits
            string url = $"https://api.github.com/users/{owner}/repos";
            string response = await GetGitHubMessage(url, clientId, clientSecret);
            if (string.IsNullOrEmpty(response) == false)
            {
                result = JsonSerializer.Deserialize<JsonElement>(response);
            }
            return result;
        }

        public async static Task<string> GetGitHubMessage(string url, string clientId, string clientSecret)
        {
            Console.WriteLine($"Running GitHub url: {url}");
            string responseBody = "";
            if (url.IndexOf("api.github.com") == -1)
            {
                throw new Exception("api.github.com missing from URL");
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TechDebtId", "1.0"));
                //If we use a id/secret, we significantly increase the rate from 60 requests an hour to 5000. https://developer.github.com/v3/#rate-limiting
                if (string.IsNullOrEmpty(clientId) == false && string.IsNullOrEmpty(clientSecret) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", clientId, clientSecret))));
                }
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    //Throw a response exception
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine(responseBody);
                    }
                }
            }
            return responseBody;
        }

    }
}
