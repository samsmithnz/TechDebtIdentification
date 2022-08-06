using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TechDebtID.Core.Models;

namespace TechDebtID.Core
{
    public class RepoScanner
    {
        public ScanSummary ScanRepo(IProgress<ProgressMessage> progress, CancellationToken cancellationToken,
            string rootFolder, bool includeTotal = true, string? outputFile = null)
        {
            int projectCount = 0;

            //scan all projects
            List<Project> projects = new List<Project>();
            int i = 0;
            foreach (DirectoryInfo folder in new DirectoryInfo(rootFolder).GetDirectories())
            {
                i++;
                projects.AddRange(DotnetProjectScanning.SearchFolderForProjectFiles(folder.FullName));
                if (projectCount != projects.Count)
                {
                    projectCount = projects.Count;
                    if (progress != null)
                    {
                        progress.Report(new ProgressMessage { ProjectsProcessed = projectCount, RootProjectsProcessed = i });
                    }
                }
            }

            //Aggregate results
            List<FrameworkSummary> frameworkSummary = AggregateFrameworks(projects, includeTotal);
            List<LanguageSummary> languageSummary = AggregateLanguages(projects, includeTotal);

            //Create an output CSV file
            if (string.IsNullOrEmpty(outputFile) == false)
            {
                OutputDataToCSVFile(projects, outputFile);
            }

            //Setup the scan summary
            ScanSummary scanSummary = new ScanSummary
            {
                //ReposCount = 0,// new DirectoryInfo(rootFolder).GetDirectories().Length,
                ProjectCount = projectCount,
                FrameworkSummary = frameworkSummary,
                LanguageSummary = languageSummary
            };
            return (scanSummary);

        }

        public List<FrameworkSummary> AggregateFrameworks(List<Project> projects, bool includeTotal)
        {
            int total = 0;
            List<FrameworkSummary> frameworkSummary = new List<FrameworkSummary>();
            foreach (Project project in projects)
            {
                project.FrameworkFamily = DotnetProjectScanning.GetFrameworkFamily(project.Framework);
                if (project.Framework == null)
                {
                    project.Framework = "(Unknown framework)";
                }

                //Process each indvidual framework
                FrameworkSummary framework = frameworkSummary.Find(i => i.Framework == project.Framework);

                //If this framework isn't in the current list, create a new one
                if (framework == null)
                {
                    frameworkSummary.Add(new FrameworkSummary
                    {
                        Framework = project.Framework,
                        FrameworkFamily = project.FrameworkFamily,
                        Count = 1 //it's the first time, start with a count of 1
                    });
                }
                else
                {
                    //There is an existing entry, increment the count
                    framework.Count++;
                }
                total++;
            }
            List<FrameworkSummary> sortedFrameworks = frameworkSummary.OrderBy(o => o.Framework).ToList();
            if (includeTotal == true)
            {
                sortedFrameworks.Add(new FrameworkSummary { Framework = "total frameworks", Count = total });
            }
            return sortedFrameworks;
        }

        public List<LanguageSummary> AggregateLanguages(List<Project> projects, bool includeTotal)
        {
            int total = 0;
            List<LanguageSummary> languageSummary = new List<LanguageSummary>();
            foreach (Project project in projects)
            {
                if (project.Language == null)
                {
                    project.Language = "(Unknown language)";
                }

                //Process each indvidual language
                LanguageSummary language = languageSummary.Find(i => i.Language == project.Language);

                //If this language isn't in the current list, create a new one
                if (language == null)
                {
                    languageSummary.Add(new LanguageSummary
                    {
                        Language = project.Language,
                        Count = 1 //it's the first time, start with a count of 1
                    });
                }
                else
                {
                    //There is an existing entry, increment the count
                    language.Count++;
                }
                total++;
            }
            List<LanguageSummary> sortedLanguages = languageSummary.OrderBy(o => o.Language).ToList();
            if (includeTotal == true)
            {
                sortedLanguages.Add(new LanguageSummary { Language = "total languages:", Count = total });
            }
            return sortedLanguages;
        }

        //private void OutputProjectDetails(List<Project> projects)
        //{
        //    foreach (Project item in projects)
        //    {
        //        Debug.WriteLine(item.Framework + ": " + item.Language + ": " + item.Path);
        //    }
        //}

        public void OutputDataToCSVFile(List<Project> projects, string filePath)
        {
            //before your loop
            StringBuilder csv = new StringBuilder();
            //Add header
            string headerLine = string.Format("{0},{1},{2}", "project file location", "framework", "language");
            csv.AppendLine(headerLine);
            //Add the rows
            foreach (Project item in projects)
            {
                string path = item.Path;
                string framework = item.Framework;
                string family = item.FrameworkFamily;
                string language = item.Language;
                string newLine = string.Format("{0},{1},{2},{3}", path, framework, family, language);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(filePath, csv.ToString());
        }

      
    }
}
