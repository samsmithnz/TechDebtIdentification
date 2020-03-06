using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TechDebtIdentification.Core;
using TechDebtIdentification.Core.Statistics;

namespace TechDebtIdentification.Tests
{
    [TestClass]
    public class RepoScannerTests
    {
        [TestMethod]
        public void ProcessRepoTest()
        {

            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            string repoToScan = @"C:\Users\samsmit\source\repos";

            //Act
            List<ProjectSummary> results = repoScanner.ProcessRepo(repoToScan);

            //Asset
            Assert.IsTrue(results.Count > 0);

        }     
        
        [TestMethod]
        public void ProcessFolderTest()
        {

            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            string folderToScan = @"C:\Users\samsmit\source\repos\TechDebtIdentification";

            //Act
            List<Project> results = repoScanner.SearchFolderForProjectFiles(folderToScan);

            //Asset
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void AggregateFrameworksTest()
        {

            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            List<Project> projects = new List<Project>
            {
                new Project
                {
                    Framework = "framework1",
                    Language = "csharp"
                },
                new Project
                {
                    Framework = "framework1",
                    Language = "csharp"
                },
                new Project
                {
                    Framework = "framework2",
                    Language = "csharp"
                }
            };

            //Act
            List<ProjectSummary> results = repoScanner.AggregateFrameworks(projects);

            //Asset
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Find(i => i.Framework == "framework1:csharp").Count == 2);
            Assert.IsTrue(results.Find(i => i.Framework == "framework2:csharp").Count == 1);
        }
    }
}
