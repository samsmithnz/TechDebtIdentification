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
        [TestMethod]
        public void ProcessRepoTest()
        {

            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            string repoToScan = @"C:\Users\samsmit\source";

            //Act
            Tuple<List<Project>, List<FrameworkSummary>> results = repoScanner.ScanRepo(repoToScan);

            //Asset
            Assert.IsTrue(results.Item1.Count > 0);
            Assert.IsTrue(results.Item2.Count > 0);
            foreach (Project item in results.Item1)
            {
                if (string.IsNullOrEmpty(item.Framework) == true)
                {
                    Debug.WriteLine(item.ToString());
                }
            }


        }

        [TestMethod]
        public void ProcessFolderTest()
        {

            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            string folderToScan = @"C:\Users\samsmit\source";

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
            List<FrameworkSummary> results = repoScanner.AggregateFrameworks(projects);

            //Asset
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Find(i => i.Framework == "framework1:csharp").Count == 2);
            Assert.IsTrue(results.Find(i => i.Framework == "framework2:csharp").Count == 1);
        }
    }
}
