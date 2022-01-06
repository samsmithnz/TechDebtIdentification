using System.Collections.Generic;

namespace TechDebtID.Core.Models
{
    public class ScanSummary
    {
        //public int ReposCount { get; set; }
        public int ProjectCount { get; set; }
        public List<FrameworkSummary> FrameworkSummary { get; set; }
        public List<LanguageSummary> LanguageSummary { get; set; }
    }
}
