using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Models;
using InterviewTest.Services;

namespace InterviewTest.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly char _hostsToken = 'H';
        private readonly char _tripsToken = 'T';

        public ActionResult Create(int count)
        {
            var db = GetDatabase();

            NewsletterService nService = new NewsletterService(db);
            nService.SaveNewsletters(_hostsToken, _tripsToken, count);

            TempData["notification"] = $"Created {count} newsletters";

            return RedirectToAction("list");
        }

        public ActionResult DeleteAll()
        {
            GetDatabase().DeleteAll<Newsletter>();

            TempData["notification"] = "All newsletters deleted";

            return RedirectToAction("list");
        }

        public ActionResult Display(string id)
        {
            var db = GetDatabase();

            var newsletter = db.Get<Newsletter>(id);
            var newsletterVmService = new NewsletterVmService(db);

            var viewModel = newsletterVmService.GetNewsletterViewModel(newsletter);

            return View("Newsletter", viewModel);
        }

        public ActionResult Sample()
        {
            var newsletterService = new NewsletterVmService(GetDatabase());
            var viewModel = newsletterService.GetSampleNewsletter();

            return View("Newsletter", viewModel);
        }

        public ActionResult List()
        {
            var viewModel = new NewsletterListViewModel
            {
                Newsletters = GetDatabase().GetAll<Newsletter>(),
            };
            ViewBag.Config = GetDatabase().GetAll<ConfigModel>() != null
                ? GetDatabase().GetAll<ConfigModel>().OrderByDescending(x => x.Id).First().ToString()
                : null;

            return View(viewModel);
        }

        public ActionResult Configure()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Configure(ConfigModel model)
        {
            if (ModelState.IsValid)
            {
                GetDatabase().Save(model);

                return Json(new {success = "true"});
            }
            return Json(new { success = "false" });
        }
        
        private FileSystemDatabase GetDatabase() => new FileSystemDatabase();
    }

    public class NewsletterViewModel
    {
        public List<object> Items { get; set; } = new List<object>();
    }

    public class NewsletterTripViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Country { get; set; }
        public string HostName { get; set; }
        public string ImageUrl { get; set; }
    }

    public class NewsletterHostViewModel
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string ImageUrl { get; set; }
    }

    public class NewsletterListViewModel
    {
        public List<Newsletter> Newsletters { get; set; }
    }
}