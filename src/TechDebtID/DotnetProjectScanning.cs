using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechDebtID.Core.Models;

namespace TechDebtID
{
    public static class DotnetProjectScanning
    {

        public static string? GetFrameworkFamily(string framework)
        {
            if (framework == null)
            {
                return null;
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
            else if (framework.StartsWith("net")) //net5.0, net6.0, etc
            {
                return ".NET";
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

        public static List<Project> SearchFolderForProjectFiles(string folder)
        {
            List<Project> projects = new();
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

            return projects;
        }  //Process .NET Framework and Core project files
        private static List<Project> ProcessDotNetProjectFile(string filePath, string language)
        {
            string[] lines = File.ReadAllLines(filePath);

            List<Project> projects = new();

            //Setup the project object
            Project project = new()
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
                            Project additionalProject = new()
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
                    project.Framework = GetHistoricalFrameworkVersion(line);
                }
                else if (line.IndexOf("ProductVersion = ") > 0)
                {
                    //Since product version could appear first in the list, and we could still find a target version, don't break out of the loop
                    project.Framework = GetHistoricalFrameworkVersion(line);
                }
            }
            //if (project.Framework == null)
            //{
            //    Console.Write("unknown framework");
            //}
            projects.Add(project);
            return projects;
        }

        private static string? GetHistoricalFrameworkVersion(string line)
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
