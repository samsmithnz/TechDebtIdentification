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
        public List<ProjectSummary> ProcessRepo(string rootFolder)
        {
            List<Project> projects = new List<Project>();
            foreach (DirectoryInfo folder in new DirectoryInfo(rootFolder).GetDirectories())
            {
                projects.AddRange(SearchFolderForProjectFiles(folder.FullName));
            }

            List<ProjectSummary> projectSummary = AggregateFrameworks(projects);
            return projectSummary;
        }

        public List<ProjectSummary> AggregateFrameworks(List<Project> projects)
        {
            List<ProjectSummary> projectSummary = new List<ProjectSummary>();
            foreach (Project project in projects)
            {
                //Search for the projectsummary entry for this framework
                ProjectSummary currentSummary = projectSummary.Find(i => i.Framework == project.Framework + ":" + project.Language);

                //If there is no project summary entry, create one
                if (currentSummary == null)
                {
                    projectSummary.Add(new ProjectSummary
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
            List<ProjectSummary> sortedProjectSummary = projectSummary.OrderBy(o => o.Framework).ToList();
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
                        projects.Add(ProcessDotNetProjectFile(fileInfo.FullName, "csharp"));
                        break;
                    case ".vbproj":
                        projects.Add(ProcessDotNetProjectFile(fileInfo.FullName, "vbdotnet"));
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
                Debug.WriteLine(item.Framework + ": " + item.Path);
            }
        }

        //Process .NET Framework and Core project files
        private Project ProcessDotNetProjectFile(string filePath, string language)
        {
            string[] lines = File.ReadAllLines(filePath);

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
                }
                else if (line.IndexOf("<TargetFramework>") > 0)
                {
                    project.Framework = line.Replace("<TargetFramework>", "").Replace("</TargetFramework>", "").Trim();
                }
            }
            return project;
        }
    }
}
