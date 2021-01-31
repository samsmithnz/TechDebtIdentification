using CommandLine;

namespace TechID
{
    public class Options
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder to search for projects")]
        public bool Folder { get; set; }
        [Option('t', "totals", Required = false, Default = false, HelpText = "Include totals in results")]
        public bool IncludeTotals { get; set; }
    }
}
