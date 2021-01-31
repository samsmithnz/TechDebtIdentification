using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TechDebtIdentification.Core.Statistics;

namespace TechDebtIdentification.Core
{
    public class RepoScanner
    {
        public async Task<ScanSummary> ScanRepo(IProgress<int> progress, CancellationToken cancellationToken, 
            string rootFolder, bool includeTotal = true)
        {
            int projectCount = 0;
            await Task.Delay(1000, cancellationToken);

            //scan all projects
            List<Project> projects = new List<Project>();
            foreach (DirectoryInfo folder in new DirectoryInfo(rootFolder).GetDirectories())
            {
                projects.AddRange(SearchFolderForProjectFiles(folder.FullName));
                if (projectCount != projects.Count)
                {
                    projectCount = projects.Count;
                    progress?.Report(projectCount);
                }
            }

            //Aggregate results
            List<FrameworkSummary> frameworkSummary = AggregateFrameworks(projects, includeTotal);
            List<LanguageSummary> languageSummary = AggregateLanguages(projects, includeTotal);

            //Setup the scan summary
            ScanSummary scanSummary = new ScanSummary
            {
                ReposCount = new DirectoryInfo(rootFolder).GetDirectories().Length,
                ProjectCount = projectCount,
                FrameworkSummary = frameworkSummary,
                LanguageSummary = languageSummary
            };
            return (scanSummary);

        }

        public ScanSummary ScanRepo(string rootFolder, bool includeTotal)
        {
            //scan all projects
            List<Project> projects = new List<Project>();
            foreach (DirectoryInfo folder in new DirectoryInfo(rootFolder).GetDirectories())
            {
                projects.AddRange(SearchFolderForProjectFiles(folder.FullName));
            }
            //Aggregate results
            int projectCount = projects.Count;
            List<FrameworkSummary> frameworkSummary = AggregateFrameworks(projects, includeTotal);
            List<LanguageSummary> languageSummary = AggregateLanguages(projects, includeTotal);

            //Setup the scan summary
            ScanSummary scanSummary = new ScanSummary
            {
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
                        projects.Add(new Project { Path = fileInfo.FullName, Language = "vb6" });
                        break;
                }
            }

            OutputProjectDetails(projects);

            return projects;
        }

        private void OutputProjectDetails(List<Project> projects)
        {
            foreach (Project item in projects)
            {
                Debug.WriteLine(item.Framework + ": " + item.Language + ": " + item.Path);
            }
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
                ////make sure we don't process old files
                //bool processProjectFile = true;
                //if (line.IndexOf("<ProductVersion>") > 0)
                //{
                //    string projectVersion = line.Replace("<ProductVersion>", "").Replace("</ProductVersion>", "").Trim();
                //    string[] projectVersionArray = projectVersion.Split('.');
                //    if (projectVersionArray.Length > 0)
                //    {
                //        if (int.TryParse(projectVersionArray[0], out int result) == true)
                //        {
                //            //Filter out projects that are earlier than Visual Studio 2010
                //            //TOOD: Don't do this, it defeats the purpose of the project to identify really old stuff
                //            if (result < 10)
                //            {
                //                processProjectFile = false;
                //            }
                //        }
                //    }
                //}

                //if (processProjectFile == true)
                //{
                if (line.IndexOf("<TargetFrameworkVersion>") > 0)
                {
                    project.Framework = line.Replace("<TargetFrameworkVersion>", "").Replace("</TargetFrameworkVersion>", "").Trim();
                }
                else if (line.IndexOf("<TargetFramework>") > 0)
                {
                    project.Framework = line.Replace("<TargetFramework>", "").Replace("</TargetFramework>", "").Trim();
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
                }
                //}
            }
            projects.Add(project);
            return projects;
        }
    }
}
