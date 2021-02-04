using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TechDebtID.Core;
using TechDebtID.Core.Statistics;

namespace TechDebtID.Tests
{
    [TestClass]
    public class RepoSyncWithStorageIntegrationTests
    {
        [TestMethod]
        public async Task GetGitHubRepoTest()
        {
            //Arrange
            GitHub repo = new GitHub();
            string organization = "samsmithnz";

            //Act
            List<string> results = await repo.GetGitHubRepos(organization);

            //Asset
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results[0] == "samsmithnz/AppSettingsYamlTest");
        }

        [TestMethod]
        public void DownloadRepoToStorageTest()
        {
            //Arrange
            IConfigurationBuilder config = new ConfigurationBuilder()
               .AddUserSecrets<RepoSyncWithStorageIntegrationTests>();
            IConfigurationRoot Configuration = config.Build();
            RepoSyncWithStorage repoSync = new RepoSyncWithStorage();
            string azureStorageConnectionString = Configuration["AzureStorageConnectionString"];
            string repo = "samsmithnz/SamsFeatureFlags";
            string destination = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                .Replace("\\TechDebtID.Tests\\bin\\Debug\\net5.0", "")
                .Replace("\\TechDebtID.Tests\\bin\\Release\\net5.0", "") + "\\GitHubTempLocation";

            //Act
            repoSync.CloneRepoToAzureStorage(azureStorageConnectionString, repo, destination);

            //Asset
            DirectoryInfo dir = new DirectoryInfo(destination);
            Assert.IsTrue(dir.GetFiles().Length > 0);
        }


    }
}
