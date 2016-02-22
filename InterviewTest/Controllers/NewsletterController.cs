using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;

namespace InterviewTest.Controllers
{
    public class NewsletterController : Controller
    {
        public ActionResult Create(int count, string newsLetterFormat)
        {
            var db = GetDatabase();

            var cleanNewsLetterFormat = Regex.Replace(newsLetterFormat.ToUpper(), "[^HT]", "");

            var numOfHosts = cleanNewsLetterFormat.Count(x => x == Constants.HostIdentifier);
            var numOfTrips = cleanNewsLetterFormat.Count(x => x == Constants.TripIdentifier);

            for (var i = 0; i < count; i++)
            {
                var newsletter = new Newsletter()
                {
                    HostIds = ReturnHostIdAndUpdateCounts(numOfHosts),
                    TripIds = ReturnTripIdAndUpdateCounts(numOfTrips),
                    NewsLetterFormat = cleanNewsLetterFormat
                };

                db.Save(newsletter);
            }

            // Updating the setting for the News Letter Format
            var setting = 
                db.GetAll<Settings>()
                    .FirstOrDefault(x => x.Key == Constants.SettingKeys.NewsLetterFormatKey) ??
                          new Settings(Constants.SettingKeys.NewsLetterFormatKey);

            setting.Value = cleanNewsLetterFormat;
            db.Save(setting);

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

            var viewModel = new NewsletterViewModel
            {
                NewsLetterFormat = newsletter.NewsLetterFormat
            };

            var hostCount = 0;
            var tripCount = 0;
            foreach (var c in newsletter.NewsLetterFormat)
            {
                switch (c)
                {
                    case Constants.HostIdentifier:
                        viewModel.Items.Add(Convert(db.Get<Host>(newsletter.HostIds[hostCount++])) as object);
                        break;

                    case Constants.TripIdentifier:
                        viewModel.Items.Add(Convert(db.Get<Trip>(newsletter.TripIds[tripCount++])) as object);
                        break;
                }
            }

            return View("Newsletter", viewModel);
        }

        public ActionResult Sample()
        {
            var viewModel = new NewsletterViewModel()
            {
                Items =
                {
                    Convert(EntityGenerator.GenerateHost()),
                    Convert(EntityGenerator.GenerateTrip(null), "Test host name"),
                    Convert(EntityGenerator.GenerateTrip(null), "Test host name"),
                    Convert(EntityGenerator.GenerateHost()),
                }
            };

            return View("Newsletter", viewModel);
        }

        public ActionResult List()
        {
            var viewModel = new NewsletterListViewModel
            {
                Newsletters = GetDatabase().GetAll<Newsletter>(),
            };

            var db = GetDatabase();
            var setting = db.GetAll<Settings>()
                .FirstOrDefault(x => x.Key == Constants.SettingKeys.NewsLetterFormatKey);

            ViewBag.NewsLetterFormat = setting == null 
                ? Constants.DefaultInitValues[Constants.SettingKeys.NewsLetterFormatKey] 
                : setting.Value;

            return View(viewModel);
        }

        private List<string> ReturnHostIdAndUpdateCounts(int count)
        {
            var db = GetDatabase();
            var hosts = db.GetAll<Host>().OrderBy(x => x.NumberOfTimesUsed).ToList();

            var result = new List<string>();
            for (var i = 0; i < count; i++)
            {
                result.Add(hosts[i].Id);

                hosts[i].NumberOfTimesUsed ++;
                db.Save(hosts[i]);
            }

            return result;
        }

        private List<string> ReturnTripIdAndUpdateCounts(int count)
        {
            var db = GetDatabase();
            var trips = db.GetAll<Trip>().OrderBy(x => x.NumberOfTimesUsed).ToList();

            var result = new List<string>();
            for (var i = 0; i < count; i++)
            {
                result.Add(trips[i].Id);

                trips[i].NumberOfTimesUsed++;
                db.Save(trips[i]);
            }

            return result;
        }

        private NewsletterHostViewModel Convert(Host host) => new NewsletterHostViewModel
        {
            Name = host.Name,
            ImageUrl = host.ImageUrl,
            Job = host.Job,
        };
        
        private NewsletterTripViewModel Convert(Trip trip, string hostName = null) => new NewsletterTripViewModel
        {
            Name = trip.Name,
            Country = trip.Country,
            HostName = hostName ?? GetDatabase().Get<Host>(trip.HostId)?.Name,
            ImageUrl = trip.ImageUrl
        };

        private FileSystemDatabase GetDatabase() => new FileSystemDatabase();
        
    }

    public class NewsletterViewModel
    {
        public string NewsLetterFormat { get; set; }
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