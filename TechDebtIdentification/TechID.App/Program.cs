using CommandLine;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TechDebtIdentification.Core;
using TechDebtIdentification.Core.Statistics;


namespace TechID
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //process arguments
            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);

            //Run the task
            if (string.IsNullOrEmpty("f") == false)
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
                Console.WriteLine("Repos searched: " + scanSummary.ReposCount);
                Console.WriteLine("Project files found: " + scanSummary.ProjectCount);

                Console.WriteLine("======================================");
                Console.WriteLine("Unique frameworks found: " + scanSummary.FrameworkSummary.Count);
                ConsoleTable
                    .From<FrameworkSummary>(scanSummary.FrameworkSummary)
                    .Configure(o => o.NumberAlignment = Alignment.Right)
                    .Write(Format.Minimal);

                Console.WriteLine("======================================");
                Console.WriteLine("Unique languages found: " + scanSummary.LanguageSummary.Count);
                ConsoleTable
                    .From<LanguageSummary>(scanSummary.LanguageSummary)
                    .Configure(o => o.NumberAlignment = Alignment.Right)
                    .Write(Format.Minimal);

                Console.ReadKey();
            }
        }

        static void RunOptions(Options opts)
        {
            //handle options
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
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
