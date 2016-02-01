using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Models;
using InterviewTest.Helpers;

namespace InterviewTest.Controllers
{
    public class NewsletterController : Controller
    {
        public ActionResult Create(int count)
        {
            var db = GetDatabase();
            
            var settings = SettingsHelper.GetSettings();
            var totalHosts = settings.Layout.Count(x => x == NewsletterItem.Host);
            var totalTrips = settings.Layout.Count - totalHosts;
            var stats = db.GetAll<Stats>().LastOrDefault();
            
            for (int i = 0; i < count; i++)
            {
                var newsletter = new Newsletter()
                {
                    HostIds = GetFairAllocation(stats, NewsletterItem.Host, totalHosts),
                    TripIds = GetFairAllocation(stats, NewsletterItem.Trip, totalTrips),
                    SettingsId = settings.Id
                };

                db.Save(newsletter);
                db.Save(stats);
            }

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
            var settings = db.Get<Settings>(newsletter.SettingsId);

            var viewModel = new NewsletterViewModel();
            var tidx = 0;
            var hidx = 0;

            foreach (var item in settings.Layout)
            {
                if (item == NewsletterItem.Trip)
                {
                    var tid = newsletter.TripIds.ElementAt(tidx++);
                    viewModel.Items.Add(Convert(db.Get<Trip>(tid)));
                }
                else
                {
                    var hid = newsletter.HostIds.ElementAt(hidx++);
                    viewModel.Items.Add(Convert(db.Get<Host>(hid)));
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

            return View(viewModel);
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
            ImageUrl = trip.ImageUrl,
        };

        private List<string> GetFairAllocation(Stats stats, NewsletterItem type, int total)
        {
            var featured = type == NewsletterItem.Trip ? stats.Trips : stats.Hosts;
            var allocation = featured.OrderBy(x => x.Value).Select(x => x.Key).Take(total).ToList();

            foreach(var id in allocation)
            {
                if(type == NewsletterItem.Trip)
                {
                    stats.Trips[id]++;
                }
                else
                {
                    stats.Hosts[id]++;
                }
            }

            return allocation;
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