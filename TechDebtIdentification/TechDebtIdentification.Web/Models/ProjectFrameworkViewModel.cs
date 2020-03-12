using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechDebtIdentification.Core.Statistics;

namespace TechDebtIdentification.Web.Models
{
    public class IndexViewModel : ScanSummary
    {
        public List<string> GraphLabels { get; set; }

        public List<string> GraphData { get; set; }
    }
}
