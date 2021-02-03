using CommandLine;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TechDebtID.Core;
using TechDebtID.Core.Statistics;


namespace TechID
{
    class Program
    {
        private static string _folder;
        private static bool _includeTotals;
        private static string _outputFile;
        private static string _GitHubOrganization;

        static async Task Main(string[] args)
        {
            //process arguments
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(RunOptions)
                   .WithNotParsed(HandleParseError);

            //Run the task
            if (string.IsNullOrEmpty(_folder) == false)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                RepoScanner repo = new RepoScanner();
                IProgress<int> progress = new Progress<int>(ReportProgress);
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                ScanSummary scanSummary = null;
               
                //do the work
                try
                {
                    scanSummary = await repo.ScanRepo(progress, tokenSource.Token, _folder, _includeTotals, _outputFile);
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
                Console.WriteLine("GitHub repo scanned: " + _GitHubOrganization);
                if (scanSummary != null)
                {
                    //Console.WriteLine("Repos searched: " + scanSummary.ReposCount);
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
                }

            }
        }

        //static string GetFolderFromArguments(string[] args)
        //{
        //    for (int i = 0; i < args.Length; i++)
        //    {
        //        if (args[i] == "-f" && i + 1 <= args.Length)
        //        {
        //            return args[i + 1];
        //        }
        //    }
        //    return "";
        //}

        static void RunOptions(Options opts)
        {
            //handle options
            _folder = opts.Folder;
            _includeTotals = opts.IncludeTotals;
            _outputFile = opts.OutputFile;
            _GitHubOrganization = opts.GitHubOrganization;
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
