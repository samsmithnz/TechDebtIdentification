using CommandLine;

namespace TechID
{
    public class Options
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder to search for projects")]
        public string Folder { get; set; }
        [Option('t', "totals", Required = false, Default = false, HelpText = "Include totals in results")]
        public bool IncludeTotals { get; set; }   
        [Option('o', "output", Required = false, HelpText = "output file to create csv file")]
        public string OutputFile { get; set; } 

        [Option('g', "GitHub organization", Required = false, HelpText = "GitHub organization to download public repos from")]
        public string GitHubOrganization { get; set; }
    }
}
