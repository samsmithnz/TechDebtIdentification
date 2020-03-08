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
            RepoScanner repo = new RepoScanner();
            Tuple<List<Project>,List<FrameworkSummary>> projectSummaries = repo.ScanRepo(@"C:\Users\samsmit\source");

            List<string> labels = new List<string>();
            List<string> data = new List<string>();
            foreach (FrameworkSummary item in projectSummaries.Item2)
            {
                labels.Add(item.Framework);
                data.Add(item.Count.ToString());
            }

            Tuple<List<string>, List<string>> chartData = new Tuple<List<string>, List<string>>(labels, data);

            return View(chartData);
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
