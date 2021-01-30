using System;
using System.Collections.Generic;
using System.Text;

namespace TechDebtIdentification.Core.Statistics
{
    public class ScanSummary
    {
        private List<FrameworkSummary> _frameworkSummary;
        private List<LanguageSummary> _languageSummary;

        public int ProjectCount { get; set; }
        public int FrameworkCount { get; set; }
        public List<FrameworkSummary> FrameworkSummary
        {
            get
            {
                return _frameworkSummary;
            }
            set
            {
                _frameworkSummary = value;
                FrameworkCount = value.Count;
            }
        }
        public int LanguageCount { get; set; }
        public List<LanguageSummary> LanguageSummary
        {
            get
            {
                return _languageSummary;
            }
            set
            {
                _languageSummary = value;
                LanguageCount = value.Count;
            }
        }
    }
}
