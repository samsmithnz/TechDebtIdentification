using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TechDebtID.Core;
using TechDebtID.Core.Statistics;

namespace TechDebtID.Tests
{
    [TestClass]
    public class RepoScannerIntegrationTests
    {

        [TestMethod]
        public async Task RepoScannerIntegrationTest()
        {
            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            IProgress<int> progress = new Progress<int>();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            bool includeTotal = true;
            string rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\TechDebtID.Tests\\bin\\Debug\\net5.0", "").Replace("\\TechDebtID.Tests\\bin\\Release\\net5.0", "");

            //Act
            ScanSummary results = await repoScanner.ScanRepo(progress, tokenSource.Token, rootFolder, includeTotal, "results.csv");

            //Asset
            Assert.AreEqual(3, results.ProjectCount);
            Assert.AreEqual(3, results.FrameworkSummary.Count);
            Assert.AreEqual(2, results.LanguageSummary.Count);
            string csv = null;
            using (var sr = new StreamReader("results.csv"))
            {
                 csv = sr.ReadToEnd();
            }
            Assert.IsNotNull(csv);
            Assert.IsTrue(csv.Length > 0);
            //TODO: Add checks to confirm contents are as expected
        }


        [TestMethod]
        public async Task RepoScannerSamplesIntegrationTest()
        {
            //Arrange
            RepoScanner repoScanner = new RepoScanner();
            IProgress<int> progress = new Progress<int>();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            bool includeTotal = true;
            string rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\TechDebtID.Tests\\bin\\Debug\\net5.0", "").Replace("\\TechDebtID.Tests\\bin\\Release\\net5.0", "") + "\\Samples";

            //Act
            ScanSummary results = await repoScanner.ScanRepo(progress, tokenSource.Token, rootFolder, includeTotal, "results.csv");

            //Asset
            Assert.AreEqual(3, results.ProjectCount);
            Assert.AreEqual(4, results.FrameworkSummary.Count);
            Assert.AreEqual(3, results.LanguageSummary.Count);
            string csv = null;
            using (var sr = new StreamReader("results.csv"))
            {
                csv = sr.ReadToEnd();
            }
            Assert.IsNotNull(csv);
            Assert.IsTrue(csv.Length > 0);
            //TODO: Add checks to confirm contents are as expected
        }

    }
}
