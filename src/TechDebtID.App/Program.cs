using CommandLine;
using ConsoleTables;
using ShellProgressBar;
using System.Diagnostics;
using TechDebtID.Core;
using TechDebtID.Core.Models;


namespace TechID
{
    class Program
    {
        private static string _folder;
        private static bool _includeTotals;
        private static string _outputFile;
        private static string _GitHubOrganization;
        private static ProgressBar progressBar;

        static void Main(string[] args)
        {
            //process arguments
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(RunOptions)
                   .WithNotParsed(HandleParseError);

            //If there is a folder to scan, run the process against it
            if (string.IsNullOrEmpty(_folder) == false)
            {
                //Initialization/ start the timer!
                Stopwatch timer = new Stopwatch();
                timer.Start();
                RepoScanner repo = new RepoScanner();
                IProgress<ProgressMessage> progress = new Progress<ProgressMessage>(ReportProgress);
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                ScanSummary scanSummary = null;
                //setup the progress bar
                int totalProgressBarTicks = new DirectoryInfo(_folder).GetDirectories().Length;
                ProgressBarOptions options = new ProgressBarOptions
                {
                    ProgressCharacter = '─',
                    ProgressBarOnBottom = true
                };
                progressBar = new ProgressBar(totalProgressBarTicks, "Searching for project files...", options);

                //start processing the work
                try
                {
                    scanSummary = repo.ScanRepo(progress, tokenSource.Token, _folder, _includeTotals, _outputFile);
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine(ex.Message, "Canceled");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, "Error");
                }

                //Show the results
                ReportProgress(new ProgressMessage());
                Console.WriteLine("Processed in " + timer.Elapsed.ToString());
                Console.WriteLine("GitHub repo scanned: " + _GitHubOrganization);
                if (scanSummary != null)
                {
                    //Console.WriteLine("Repos searched: " + scanSummary.ReposCount);
                    Console.WriteLine("Project files found: " + scanSummary.ProjectCount);

                    Console.WriteLine("======================================");
                    Console.WriteLine("Unique frameworks: " + (scanSummary.FrameworkSummary.Count - 1).ToString());
                    ConsoleTable
                        .From<FrameworkSummary>(scanSummary.FrameworkSummary)
                        .Configure(o => o.NumberAlignment = Alignment.Right)
                        .Write(Format.Minimal);

                    Console.WriteLine("======================================");
                    Console.WriteLine("Unique languages: " + (scanSummary.LanguageSummary.Count - 1).ToString());
                    ConsoleTable
                        .From<LanguageSummary>(scanSummary.LanguageSummary)
                        .Configure(o => o.NumberAlignment = Alignment.Right)
                        .Write(Format.Minimal);
                }

            }
        }

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

        private static void ReportProgress(ProgressMessage message)
        {
            //Console.Clear();
            if (message.ProjectsProcessed != 0)
            {
                //progressBar.Report(0.25);
                progressBar.Tick(message.RootProjectsProcessed, $"Scanning for projects... (projects found: {message.ProjectsProcessed})");
                //Console.WriteLine($"Scanning for projects... (projects found: {number})");
            }
            else
            {
                Console.WriteLine("");
            }
        }
    }
}
