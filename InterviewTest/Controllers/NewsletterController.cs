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
        public ActionResult Create(int count, string newsletterFormat)
        {
            var db = GetDatabase();

            var hostIds = db.GetAll<Host>().Select(h => h.Id).ToArray();
            var tripIds = db.GetAll<Trip>().Select(t => t.Id).ToArray();

            for (int i = 0; i < count; i++)
            {
                var newsletter = new Newsletter()
                {
                    Format = newsletterFormat,
                    ResourceIds = new List<string>()
                };

                //look at each character in the format string, and if it's a T then get a random trip id, 
                //otherwise if a H then get a random host id
                foreach (char resourceType in newsletterFormat.ToUpper())
                {
                    if (resourceType == 'T')
                    {
                        newsletter.ResourceIds.Add(string.Format("T:{0}", tripIds.GetRandom()));
                    }
                    else if (resourceType == 'H')
                    {
                        newsletter.ResourceIds.Add(string.Format("H:{0}", hostIds.GetRandom()));
                    }
                }

                db.Save(newsletter);
            }

            //now save the format of the newsletter to the file system
            var newsletterSettings = new NewsletterSettings()
            {
                Id = "Settings",
                NewsletterFormat = newsletterFormat
            };
            db.Save(newsletterSettings);

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

            var viewModel = new NewsletterViewModel();
            viewModel.Items = new List<object>();

            foreach (string resourceId in newsletter.ResourceIds)
            {
                if (resourceId.Substring(0,2) == "T:")
                {
                    viewModel.Items.Add(Convert(db.Get<Trip>(resourceId.Substring(2))));
                }
                else if (resourceId.Substring(0, 2) == "H:")
                {
                    viewModel.Items.Add(Convert(db.Get<Host>(resourceId.Substring(2))));
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
                Settings = GetDatabase().Get<NewsletterSettings>("Settings")
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
        public NewsletterSettings Settings { get; set; }
    }
}