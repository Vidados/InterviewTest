using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;

namespace InterviewTest.Controllers
{
    public class NewsletterController : Controller
    {
        public ActionResult Create(int count)
        {
            var db = GetDatabase();

            // load saved configuration
            var sequence = "";
            var configuredSequence = db.Get<Config>("00000");

            // if no configuration found
            if (configuredSequence == null)
            {
                TempData["notification"] = "You must configure a newsletter format first";
                return RedirectToAction("list");
            }

            // if config found
            sequence = configuredSequence.Sequence;

            // eval amount of hosts/trips defined by user
            int hostsCount = 0, tripCount = 0;
            var newsletterContent = sequence.ToCharArray();
            for (int i = 0; i < newsletterContent.Length; i++)
            {
                if (newsletterContent[i] == 'H') { hostsCount++; continue; }
                if (newsletterContent[i] == 'T') { tripCount++; continue; }
            }

            // get hosts ordered by newsletter count
            var hostIds = (from h in db.GetAll<Host>()
                           orderby h.NewsletterCount
                           select h.Id).Take(hostsCount).ToArray();

            // get trips ordered by newsletter count
            var tripIds = (from t in db.GetAll<Trip>()
                           orderby t.NewsletterCount
                           select t.Id).Take(tripCount).ToArray();

            // add newsletters
            for (int i = 0; i < count; i++)
            {
                var newsletter = new Newsletter()
                {
                    HostIds = hostIds.ToList(),
                    TripIds = tripIds.ToList(),
                    Sequence = newsletterContent
                };

                db.Save(newsletter);
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

            var viewModel = new NewsletterViewModel() { Items = new List<object>() };

            // retrieve newsletter trips
            var trips = newsletter.TripIds.Select(tid => Convert(db.Get<Trip>(tid))).Cast<object>();
            int lastEvaluatedTripId = 0;

            // retrieve newsletter hosts
            var hosts = newsletter.HostIds.Select(hid => Convert(db.Get<Host>(hid))).Cast<object>();
            int lastEvaluatedHostId = 0;

            // evaluate sequence
            for (int i = 0; i < newsletter.Sequence.Length; i++)
            {
                if (newsletter.Sequence[i] == 'H') {
                    viewModel.Items.Add(hosts.ElementAt(lastEvaluatedHostId));
                    lastEvaluatedHostId++;
                }
                if (newsletter.Sequence[i] == 'T')
                {
                    viewModel.Items.Add(trips.ElementAt(lastEvaluatedTripId));
                    lastEvaluatedTripId++;
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
                Newsletters = GetDatabase().GetAll<Newsletter>()
            };

            return View(viewModel);
        }

        public ActionResult SaveConfiguration(string contentSequence)
        {
            var db = GetDatabase();
            db.DeleteAll<Config>();

            var config = new Config()
            {
                Id = "default",
                Sequence = contentSequence
            };

            db.Save<Config>(config);

            return RedirectToAction("list");
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