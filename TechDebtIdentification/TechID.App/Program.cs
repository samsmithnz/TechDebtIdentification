using System;
using TechDebtIdentification.Core;
using TechDebtIdentification.Core.Statistics;

namespace TechID.App
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan span = new TimeSpan();
            string repoLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\source\repos\SamLearnsAzure";
            RepoScanner repo = new RepoScanner();
            ScanSummary scanSummary = repo.ScanRepo(repoLocation);

            //results
            Console.WriteLine("Processed in " + span.ToString() + " seconds");
            Console.WriteLine(scanSummary.ProjectCount + " projects found");
            Console.WriteLine(scanSummary.FrameworkCount + " frameworks found");
            Console.WriteLine(scanSummary.LanguageCount + " languages found");

        }
    }
}
