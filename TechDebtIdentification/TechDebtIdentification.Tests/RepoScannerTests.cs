using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TechDebtIdentification.Core;
using TechDebtIdentification.Core.Statistics;

namespace TechDebtIdentification.Tests
{
    [TestClass]
    public class RepoScannerTests
    {
        private string repoLocation = "";

        [TestInitialize]
        public void SetupTests()
        {
            repoLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\source";
        }

        [TestMethod]
        public void ProcessRepoTest()
        {
            //Arrange
            RepoScanner repoScanner = new RepoScanner();

            //Act
            ScanSummary results = repoScanner.ScanRepo(repoLocation);

            //Asset
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.ProjectCount >= 0);
            Assert.IsTrue(results.FrameworkCount >= 0);
            Assert.IsTrue(results.FrameworkSummary.Count >= 0);
            Assert.IsTrue(results.LanguageCount >= 0);
            Assert.IsTrue(results.LanguageSummary.Count >= 0);
        }

        [TestMethod]
        public void ProcessFolderTest()
        {
            //Arrange
            RepoScanner repoScanner = new RepoScanner();

            //Act
            List<Project> results = repoScanner.SearchFolderForProjectFiles(repoLocation);

            //Asset
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void AggregateFrameworksTest()
        {
            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            List<Project> projects = GenerateSampleData();

            //Act
            List<FrameworkSummary> results = repoScanner.AggregateFrameworks(projects);

            //Asset
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Find(i => i.Framework == "framework1").Count == 2);
            Assert.IsTrue(results.Find(i => i.Framework == "framework2").Count == 1);
        }

        [TestMethod]
        public void AggregateLanguagesTest()
        {
            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            List<Project> projects = GenerateSampleData();

            //Act
            List<LanguageSummary> results = repoScanner.AggregateLanguages(projects);

            //Asset
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Find(i => i.Language == "csharp").Count == 3);
        }

        private List<Project> GenerateSampleData()
        {
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
            return projects;
        }
    }
}
