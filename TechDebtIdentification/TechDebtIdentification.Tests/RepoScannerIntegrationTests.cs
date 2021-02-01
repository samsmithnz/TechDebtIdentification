using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TechDebtIdentification.Core;
using TechDebtIdentification.Core.Statistics;

namespace TechDebtIdentification.Tests
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
            string rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\TechDebtIdentification.Tests\\bin\\Debug\\net5.0", "").Replace("\\TechDebtIdentification.Tests\\bin\\Release\\net5.0", "");

            //Act
            ScanSummary results = await repoScanner.ScanRepo(progress, tokenSource.Token, rootFolder, includeTotal, null);

            //Asset
            //Assert.AreEqual(5, results.ReposCount);
            Assert.AreEqual(3, results.ProjectCount);
            Assert.AreEqual(3, results.FrameworkSummary.Count);
            Assert.AreEqual(2, results.LanguageSummary.Count);
        }

    }
}
