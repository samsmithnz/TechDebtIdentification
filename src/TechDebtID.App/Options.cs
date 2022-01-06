using CommandLine;

namespace TechID
{
    public class Options
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder to search for projects")]
        public string Folder { get; set; }
        [Option('i', "includetotals", Required = false, Default = false, HelpText = "Include totals in results")]
        public bool IncludeTotals { get; set; }
        [Option('o', "output", Required = false, HelpText = "output file to create csv file")]
        public string OutputFile { get; set; }
        [Option('g', "githuborg", Required = false, HelpText = "GitHub organization to download public repos from. e.g: ")]
        public string GitHubOrganization { get; set; }
    }
}
