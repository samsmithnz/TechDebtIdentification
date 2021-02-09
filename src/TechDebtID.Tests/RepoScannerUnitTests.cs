using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TechDebtID.Core;
using TechDebtID.Core.Statistics;

namespace TechDebtID.Tests
{
    [TestClass]
    public class RepoScannerUnitTests
    {

        [TestMethod]
        public void AggregateFrameworksTest()
        {
            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            List<Project> projects = GenerateSampleData();
            bool includeTotal = true;

            //Act
            List<FrameworkSummary> results = repoScanner.AggregateFrameworks(projects, includeTotal);

            //Asset
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual(2, results.Find(i => i.Framework == "netstandard2.0").Count);
            Assert.AreEqual(1, results.Find(i => i.Framework == "netcoreapp3.1").Count);
            Assert.AreEqual(1, results.Find(i => i.Framework == "net45").Count);
            Assert.AreEqual(6, results[^1].Count);
        }

        [TestMethod]
        public void AggregateLanguagesTest()
        {
            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            List<Project> projects = GenerateSampleData();
            bool includeTotal = true;

            //Act
            List<LanguageSummary> results = repoScanner.AggregateLanguages(projects, includeTotal);

            //Asset
            Assert.AreEqual(5, results.Count);
            Assert.AreEqual(3, results.Find(i => i.Language == "csharp").Count);
            Assert.AreEqual(1, results.Find(i => i.Language == "vbdotnet").Count);
            Assert.AreEqual(6, results[^1].Count);
        }

        private static List<Project> GenerateSampleData()
        {
            List<Project> projects = new List<Project>
            {
                new Project
                {
                    Framework = "netstandard2.0",
                    Language = "csharp",
                    Path = @"c:\Project1"
                },
                new Project
                {
                    Framework = "netstandard2.0",
                    Language = "csharp",
                    Path = @"c:\Project2"
                },
                new Project
                {
                    Framework = "netcoreapp3.1",
                    Language = "csharp",
                    Path = @"c:\Project3"
                },
                new Project
                {
                    Framework = "net45",
                    Language = "vbdotnet",
                    Path = @"c:\Project4"
                },
                new Project
                {
                    Framework = "vb6",
                    Language = "vb6",
                    Path = @"c:\Project5"
                },
                new Project
                {
                    Framework = null,
                    Language = null,
                    Path = @"c:\Project6"
                }
            };
            return projects;
        }
    }
}
