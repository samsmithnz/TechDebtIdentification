using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechDebtIdentification.Core;
using TechDebtIdentification.Core.Statistics;
using TechDebtIdentification.Web.Models;

namespace TechDebtIdentification.Web.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            string repoLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\source";
            RepoScanner repo = new RepoScanner();
            ScanSummary scanSummary = repo.ScanRepo(repoLocation);

            //Setup data for the chart
            List<string> labels = new List<string>();
            List<string> data = new List<string>();
            foreach (FrameworkSummary item in scanSummary.FrameworkSummary)
            {
                labels.Add(item.Framework);
                data.Add(item.Count.ToString());
            }

            //populate the viewmodel with data to use on the index page
            IndexViewModel indexModel = new IndexViewModel
            {
                GraphLabels = labels,
                GraphData = data,
                FrameworkCount = scanSummary.FrameworkCount,
                FrameworkSummary = scanSummary.FrameworkSummary,
                LanguageCount = scanSummary.LanguageCount,
                LanguageSummary = scanSummary.LanguageSummary,
                ProjectCount = scanSummary.ProjectCount
            };

            return View(indexModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
