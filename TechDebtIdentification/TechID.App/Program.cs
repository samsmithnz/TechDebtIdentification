using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TechDebtIdentification.Core;
using TechDebtIdentification.Core.Statistics;


namespace TechID.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            string repoLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\source\repos";
            RepoScanner repo = new RepoScanner();
            CancellationTokenSource tokenSource;
            IProgress<int> progress = new Progress<int>(ReportProgress);
            ScanSummary scanSummary = null;
            tokenSource = new CancellationTokenSource();

            //do the work
            try
            {
                scanSummary = await repo.Get(repoLocation, progress, tokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message, "Canceled");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error");
            }

            //results
            ReportProgress(0);
            Console.WriteLine("Processed in " + timer.Elapsed.ToString());
            Console.WriteLine("Repos scanned: " + scanSummary.ReposCount);
            Console.WriteLine("Projects found: " + scanSummary.ProjectCount);
            Console.WriteLine("======================================");
            foreach (LanguageSummary item in scanSummary.LanguageSummary)
            {
                Console.WriteLine("Language:" + item.Language + ", Count: " + item.Count);
            }
            Console.WriteLine("======================================");
            Console.WriteLine("Frameworks found: " + scanSummary.FrameworkCount);
            Console.WriteLine("======================================");
            foreach (FrameworkSummary item in scanSummary.FrameworkSummary)
            {
                Console.WriteLine("Framework:" + item.Framework + ", Count: " + item.Count);
            }
            Console.WriteLine("======================================");
            Console.WriteLine("Languages found: " + scanSummary.LanguageCount);
            Console.WriteLine("======================================");
            foreach (LanguageSummary item in scanSummary.LanguageSummary)
            {
                Console.WriteLine("Language:" + item.Language + ", Count: "+ item.Count);
            }
            Console.WriteLine("======================================");
        }

        private static void ReportProgress(int number)
        {
            //Console.Clear();
            if (number != 0)
            {
                Console.WriteLine($"Scanning for projects... (projects found: {number})");
            }
            else
            {
                Console.WriteLine("");
            }
        }
    }
}
