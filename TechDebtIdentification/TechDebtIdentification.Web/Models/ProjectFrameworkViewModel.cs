using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechDebtIdentification.Web.Models
{
    public class ProjectFrameworkViewModel
    {
        [JsonProperty(PropertyName = "Framework")]
        public List<string> Framework { get; set; }

        [JsonProperty(PropertyName = "Count")]
        public List<int> Count { get; set; }
    }
}
