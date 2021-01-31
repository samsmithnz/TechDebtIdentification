using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TechDebtIdentification.Core;
using TechDebtIdentification.Core.Statistics;

namespace TechDebtIdentification.Tests
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
            Assert.IsTrue(results.Count == 4);
            Assert.IsTrue(results.Find(i => i.Framework == "framework1").Count == 2);
            Assert.IsTrue(results.Find(i => i.Framework == "framework2").Count == 1);
            Assert.IsTrue(results.Find(i => i.Framework == "framework3").Count == 1);
            Assert.IsTrue(results[^1].Count == 4);
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
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Find(i => i.Language == "csharp").Count == 3);
            Assert.IsTrue(results.Find(i => i.Language == "vbdotnet").Count == 1);
            Assert.IsTrue(results[^1].Count == 4);
        }

        private static List<Project> GenerateSampleData()
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
                },
                new Project
                {
                    Framework = "framework3",
                    Language = "vbdotnet"
                }
            };
            return projects;
        }
    }
}
