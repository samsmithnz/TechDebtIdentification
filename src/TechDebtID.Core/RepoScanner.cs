using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechDebtID.Core.Statistics;

namespace TechDebtID.Core
{
    public class RepoScanner
    {
        public async Task<ScanSummary> ScanRepo(IProgress<ProgressMessage> progress, CancellationToken cancellationToken,
            string rootFolder, bool includeTotal = true, string outputFile = null)
        {
            int projectCount = 0;
            await Task.Delay(1000, cancellationToken);

            //scan all projects
            List<Project> projects = new List<Project>();
            int i = 0;
            foreach (DirectoryInfo folder in new DirectoryInfo(rootFolder).GetDirectories())
            {
                i++;
                projects.AddRange(SearchFolderForProjectFiles(folder.FullName));
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
                project.FrameworkFamily = GetFrameworkFamily(project.Framework);
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

        private string GetFrameworkFamily(string framework)
        {
            if (framework == null)
            {
                return null;
            }
            else if (framework.StartsWith("net5.0"))
            {
                return ".NET";
            }
            else if (framework.StartsWith("netcoreapp"))
            {
                return ".NET Core";
            }
            else if (framework.StartsWith("netstandard"))
            {
                return ".NET Standard";
            }
            else if (framework.StartsWith("v1."))
            {
                return ".NET Framework";
            }
            else if (framework.StartsWith("v2."))
            {
                return ".NET Framework";
            }
            else if (framework.StartsWith("v3."))
            {
                return ".NET Framework";
            }
            else if (framework.StartsWith("v4.") || framework.StartsWith("net4"))
            {
                return ".NET Framework";
            }
            else if (framework.StartsWith("vb6"))
            {
                return "Visual Basic 6";
            }
            else
            {
                return null;
            }
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

        public List<Project> SearchFolderForProjectFiles(string folder)
        {
            List<Project> projects = new List<Project>();
            foreach (FileInfo fileInfo in new DirectoryInfo(folder).GetFiles("*.*", SearchOption.AllDirectories))
            {
                //if .NET project files are found, process them
                switch (fileInfo.Extension.ToLower())
                {
                    case ".csproj":
                        projects.AddRange(ProcessDotNetProjectFile(fileInfo.FullName, "csharp"));
                        break;
                    case ".vbproj":
                        projects.AddRange(ProcessDotNetProjectFile(fileInfo.FullName, "vb.net"));
                        break;
                    case ".vbp":
                        projects.Add(new Project { Path = fileInfo.FullName, Framework = "vb6", Language = "vb6" });
                        break;
                }
            }

            //OutputProjectDetails(projects);

            return projects;
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

        //Process .NET Framework and Core project files
        private List<Project> ProcessDotNetProjectFile(string filePath, string language)
        {
            string[] lines = File.ReadAllLines(filePath);

            List<Project> projects = new List<Project>();

            //Setup the project object
            Project project = new Project
            {
                Path = filePath,
                Language = language
            };
            //scan the project file to identify the framework
            foreach (string line in lines)
            {
                if (line.IndexOf("<TargetFrameworkVersion>") > 0)
                {
                    project.Framework = line.Replace("<TargetFrameworkVersion>", "").Replace("</TargetFrameworkVersion>", "").Trim();
                    break;
                }
                else if (line.IndexOf("<TargetFramework>") > 0)
                {
                    project.Framework = line.Replace("<TargetFramework>", "").Replace("</TargetFramework>", "").Trim();
                    break;
                }
                else if (line.IndexOf("<TargetFrameworks>") > 0)
                {
                    string frameworks = line.Replace("<TargetFrameworks>", "").Replace("</TargetFrameworks>", "").Trim();
                    string[] frameworkList = frameworks.Split(';');
                    for (int i = 0; i < frameworkList.Length - 1; i++)
                    {
                        if (i == 0)
                        {
                            project.Framework = frameworkList[i];
                        }
                        else
                        {
                            Project additionalProject = new Project
                            {
                                Path = filePath,
                                Language = language,
                                Framework = frameworkList[i]
                            };
                            projects.Add(additionalProject);
                        }
                    }
                    break;
                }
                else if (line.IndexOf("<ProductVersion>") > 0)
                {
                    //Since product version could appear first in the list, and we could still find a target version, don't break out of the loop
                    project.Framework = GetHistoricallyOldFrameworkVersion(line);
                }
                else if (line.IndexOf("ProductVersion = ") > 0)
                {
                    //Since product version could appear first in the list, and we could still find a target version, don't break out of the loop
                    project.Framework = GetHistoricallyOldFrameworkVersion(line);
                }
            }
            if (project.Framework == null)
            {
                Console.Write("unknown framework");
            }
            projects.Add(project);
            return projects;
        }

        private string GetHistoricallyOldFrameworkVersion(string line)
        {
            string productVersion = line.Replace("<ProductVersion>", "").Replace("</ProductVersion>", "").Replace("ProductVersion = ", "").Replace("\"", "").Trim();
            //https://en.wikipedia.org/wiki/Microsoft_Visual_Studio#History
            //+---------------------------+---------------+-----------+----------------+
            //|       Product name        |   Codename    | Version # | .NET Framework | 
            //+---------------------------+---------------+-----------+----------------+
            //| Visual Studio .NET (2002) | Rainier       | 7.0.*     | 1              |
            //| Visual Studio .NET 2003   | Everett       | 7.1.*     | 1.1            |
            //| Visual Studio 2005        | Whidbey       | 8.0.*     | 2.0, 3.0       |
            //| Visual Studio 2008        | Orcas         | 9.0.*     | 2.0, 3.0, 3.5  |
            //| Visual Studio 2010        | Dev10/Rosario | 10.0.*    | 2.0 – 4.0      |
            //| Visual Studio 2012        | Dev11         | 11.0.*    | 2.0 – 4.5.2    |
            //| Visual Studio 2013        | Dev12         | 12.0.*    | 2.0 – 4.5.2    |
            //| Visual Studio 2015        | Dev14         | 14.0.*    | 2.0 – 4.6      |
            //+---------------------------+---------------+-----------+----------------+

            //Only process the earliest Visual Studio's, as the later versions should be picked up by the product version
            if (productVersion.StartsWith("7.0") == true)
            {
                return "v1.0";
            }
            else if (productVersion.StartsWith("7.1") == true)
            {
                return "v1.1";
            }
            else if (productVersion.StartsWith("8.0") == true)
            {
                return "v2.0";
            }
            else
            {
                return null;
            }
        }
    }
}
