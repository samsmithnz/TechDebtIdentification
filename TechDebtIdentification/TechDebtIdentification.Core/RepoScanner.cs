using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TechDebtIdentification.Core.Statistics;

namespace TechDebtIdentification.Core
{
    public class RepoScanner
    {
        public Tuple<List<Project>, List<FrameworkSummary>> ScanRepo(string rootFolder)
        {
            List<Project> projects = new List<Project>();
            foreach (DirectoryInfo folder in new DirectoryInfo(rootFolder).GetDirectories())
            {
                projects.AddRange(SearchFolderForProjectFiles(folder.FullName));
            }

            List<FrameworkSummary> projectSummary = AggregateFrameworks(projects);
            return new Tuple<List<Project>, List<FrameworkSummary>>(projects, projectSummary);
        }

        public List<FrameworkSummary> AggregateFrameworks(List<Project> projects)
        {
            List<FrameworkSummary> projectSummary = new List<FrameworkSummary>();
            foreach (Project project in projects)
            {
                //Search for the projectsummary entry for this framework
                FrameworkSummary currentSummary = projectSummary.Find(i => i.Framework == project.Framework + ":" + project.Language);

                //If there is no project summary entry, create one
                if (currentSummary == null)
                {
                    projectSummary.Add(new FrameworkSummary
                    {
                        Framework = project.Framework + ":" + project.Language,
                        Count = 1
                    });
                }
                else
                {
                    //There is an existing entry, increment the count
                    currentSummary.Count++;
                }
            }
            List<FrameworkSummary> sortedProjectSummary = projectSummary.OrderBy(o => o.Framework).ToList();
            return sortedProjectSummary;
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
                        projects.AddRange(ProcessDotNetProjectFile(fileInfo.FullName, "vbdotnet"));
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
