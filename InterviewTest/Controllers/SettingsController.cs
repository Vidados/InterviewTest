using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Helpers;
using InterviewTest.Models;

namespace InterviewTest.Controllers
{
    public class SettingsController : Controller
    {
        public ActionResult Index()
        {
            var db = GetDatabase();

            var settings = db.GetAll<Settings>().LastOrDefault() ?? new Settings() { Layout = SettingsHelper.GetDefaultLayout() };

            var viewModel = new SettingsViewModel()
            {
                Current = new CurrentSettingsViewModel()
                {
                    Layout = settings.Layout
                },
                New = new NewSettingsViewModel()
                {
                    Total = SettingsHelper.MaxItemsAllowed
                }
            };

            return View("Config", viewModel);
        }

        [HttpPost]
        public ActionResult Save(NewSettingsViewModel model)
        {
            var settings = new Settings();
            for(int i = 1; i <= model.Total; i++)
            {
                var item = (int)model.GetType().GetProperty("Item" + i).GetValue(model, null);
                settings.Layout.Add((NewsletterItem)item);
            }

            var db = GetDatabase();
            db.Save(settings);

            TempData["notification"] = "Settings have been updated";

            return RedirectToAction("Index", "Home");
        }

        private FileSystemDatabase GetDatabase() => new FileSystemDatabase();
    }

    public class SettingsViewModel
    {
        public CurrentSettingsViewModel Current { get; set; }
        public NewSettingsViewModel New { get; set; }
    }

    public class CurrentSettingsViewModel
    {
        public IList<NewsletterItem> Layout { get; set; } = new List<NewsletterItem>();
        public int Total
        {
            get { return Layout.Count; }
        }
        public int TotalHosts
        {
            get { return Layout.Count(x => x == NewsletterItem.Host); }
        }
        public int TotalTrips
        {
            get { return Layout.Count(x => x == NewsletterItem.Trip); }
        }
    }

    public class NewSettingsViewModel
    {
        public int Total { get; set; }
        public IEnumerable<SelectListItem> ItemsAllowed
        {
            get { return SettingsHelper.GetItemsAllowed(); }
        }
        public int Item1 { get; set; }
        public int Item2 { get; set; }
        public int Item3 { get; set; }
        public int Item4 { get; set; }
        public int Item5 { get; set; }
        public int Item6 { get; set; }
        public int Item7 { get; set; }
        public int Item8 { get; set; }
        public int Item9 { get; set; }
        public int Item10 { get; set; }
        public IEnumerable<SelectListItem> NewsletterItems
        {
            get { return SettingsHelper.GetNewsletterItems(); }
        }
    }
}