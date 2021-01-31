using System;
using System.Collections.Generic;
using System.Text;

namespace TechDebtIdentification.Core.Statistics
{
    public class ScanSummary
    {
        public int ReposCount { get; set; }
        public int ProjectCount { get; set; }
        public List<FrameworkSummary> FrameworkSummary { get; set; }
        public List<LanguageSummary> LanguageSummary { get; set; }
    }
}
