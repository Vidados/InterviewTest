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
        private List<HostTripCount> _hostCounts;
        private List<HostTripCount> _tripCounts;

        private string[] _hostIds;
        private string[] _tripIds;

        private void InitialiseTripHostCounts()
        {
            //Populate the two lists which count how many times each host and trip
            //has been featured so far across all newsletters
            var db = GetDatabase();

            _hostCounts = new List<HostTripCount>();
            _tripCounts = new List<HostTripCount>();

            var hostIds = db.GetAll<Host>().Select(h => h.Id).ToArray();
            var tripIds = db.GetAll<Trip>().Select(t => t.Id).ToArray();
            var allNewsletters = GetDatabase().GetAll<Newsletter>();

            //first create an entry for each host and trip
            foreach (var hostId in hostIds)
            {
                var newHostCount = new HostTripCount()
                {
                    ResourceId = hostId,
                    Count = 0
                };
                _hostCounts.Add(newHostCount);
            }
            foreach (var tripId in tripIds)
            {
                var newTripCount = new HostTripCount()
                {
                    ResourceId = tripId,
                    Count = 0
                };
                _tripCounts.Add(newTripCount);
            }

            if (allNewsletters != null)
            {
                List<HostTripCount> listToSearch;
                foreach (var newsLetter in allNewsletters)
                {
                    foreach (var resourceId in newsLetter.ResourceIds)
                    {
                        if (resourceId.Substring(0, 2) == "T:")
                        {
                            listToSearch = _tripCounts;
                        }
                        else
                        {
                            listToSearch = _hostCounts;
                        }
                        foreach (var hostTripCount in listToSearch)
                        {
                            if (hostTripCount.ResourceId == resourceId.Substring(2))
                            {
                                hostTripCount.Count++;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private string GetHostId()
        {
            //sort the hosts by number of times they have appeared and use the lowest (if more than one the choice will effectively be random)
            List<HostTripCount> SortedHostList = _hostCounts.OrderBy(o => o.Count).ToList();
            var lowestHost = SortedHostList[0];
            //now update the count
            lowestHost.Count++;
            return lowestHost.ResourceId;
        }

        private string GetTripId()
        {
            //sort the trips by number of times they have appeared and use the lowest (if more than one the choice will effectively be random)
            List<HostTripCount> SortedTripList = _tripCounts.OrderBy(o => o.Count).ToList();
            var lowestTrip = SortedTripList[0];
            //now update the count
            lowestTrip.Count++;
            return lowestTrip.ResourceId;
        }

        public ActionResult Create(int count, string newsletterFormat)
        {
            var db = GetDatabase();

            _hostIds = db.GetAll<Host>().Select(h => h.Id).ToArray();
            _tripIds = db.GetAll<Trip>().Select(t => t.Id).ToArray();

            //to make it fairer, first get all previous newsletters and count how many times
            //each trip and host has been featured, then use the lowest count for each to generate
            //the new newsletters, this will cater for previously generated newsletters and over time
            //the counts should coverge
            InitialiseTripHostCounts();


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
                        newsletter.ResourceIds.Add(string.Format("T:{0}", GetTripId()));
                    }
                    else if (resourceType == 'H')
                    {
                        newsletter.ResourceIds.Add(string.Format("H:{0}", GetHostId()));
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