using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

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
                if (repoJson.TryGetProperty("full_name", out JsonElement jsonElement) == true)
                {
                    repos.Add(jsonElement.ToString());
                }
            }
            return repos;
        }

        public async Task<List<string>> GetGitHubRepoFiles(string organization, string repo, string defaultBranch)
        {
            List<string> files = new List<string>();
            JsonElement resultJson2 = await GetGitHubCommitFilesFromAPI("", "", organization, repo, defaultBranch);
            if (resultJson2.TryGetProperty("tree", out JsonElement jsonElementTree) == true)
            {
                foreach (JsonElement repoJson in jsonElementTree.EnumerateArray())
                {
                    if (repoJson.TryGetProperty("path", out JsonElement jsonElement) == true)
                    {
                        files.Add(jsonElement.ToString());
                    }
                }
            }
            return files;
        }


        private async Task<JsonElement> GetGitHubReposFromAPI(string clientId, string clientSecret, string owner)
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

        ////Get GitHub commit list (required to get the last commit and hence files on commit)
        //private async Task<JsonElement> GetGitHubCommitsFromAPI(string clientId, string clientSecret, string owner, string repo)
        //{
        //    JsonElement result = new JsonElement();
        //    //https://docs.github.com/en/rest/reference/repos#list-commits
        //    //GET /repos/{owner}/{repo}/commits
        //    string url = $"https://api.github.com/repos/{owner}/{repo}/commits";
        //    string response = await GetGitHubMessage(url, clientId, clientSecret);
        //    if (string.IsNullOrEmpty(response) == false)
        //    {
        //        result = JsonSerializer.Deserialize<JsonElement>(response);
        //    }
        //    return result;
        //}

        //Get GitHub commit contents (needed to get a list of files in a repo)
        private async Task<JsonElement> GetGitHubCommitFilesFromAPI(string clientId, string clientSecret, string owner, string repo, string defaultBranch)
        {
            JsonElement result = new JsonElement();
            //https://docs.github.com/en/rest/reference/git#trees
            //GET /repos/{owner}/{repo}/commits/{ref}
            string url = $"https://api.github.com/repos/{owner}/{repo}/git/trees/{defaultBranch}?recursive=1";
            string response = await GetGitHubMessage(url, clientId, clientSecret);
            if (string.IsNullOrEmpty(response) == false)
            {
                result = JsonSerializer.Deserialize<JsonElement>(response);
            }
            return result;
        }

        ////Get GitHub file content
        //private async Task<JsonElement> GetGitHubFileContentsFromAPI(string clientId, string clientSecret, string owner, string repo, string path)
        //{
        //    JsonElement result = new JsonElement();
        //    //https://docs.github.com/en/rest/reference/repos#get-repository-content
        //    //GET /repos/{owner}/{repo}/contents/{path}
        //    string url = $"https://api.github.com/repos/{owner}/{repo}/contents/{path}";
        //    string response = await GetGitHubMessage(url, clientId, clientSecret);
        //    if (string.IsNullOrEmpty(response) == false)
        //    {
        //        result = JsonSerializer.Deserialize<JsonElement>(response);
        //    }
        //    return result;
        //}

        //package up the url request, client id and secret (if exists), and send and process the request
        private async static Task<string> GetGitHubMessage(string url, string clientId, string clientSecret)
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
