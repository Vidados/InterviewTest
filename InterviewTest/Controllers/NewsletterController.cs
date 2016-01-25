using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;
using InterviewTest.Services;

namespace InterviewTest.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly INewsletterService _newsletterService;
        private readonly IHostService _hostService;
        private readonly ITripService _tripService;

        public NewsletterController(INewsletterService newsletterService, IHostService hostService, ITripService tripService)
        {
            _newsletterService = newsletterService;
            _hostService = hostService;
            _tripService = tripService;
        }

        public ActionResult Create(int count)
        {
            _newsletterService.CreateRandomData(count);
            TempData["notification"] = $"Created {count} newsletters";

            return RedirectToAction("list");
        }

        public ActionResult DeleteAll()
        {
            _newsletterService.DeleteAll();

            TempData["notification"] = "All newsletters deleted";

            return RedirectToAction("list");
        }

        public ActionResult Display(string id)
        {
            var newsletter = _newsletterService.GetById(id);

            var viewModel = new NewsletterViewModel
            {
                Items = newsletter.TripIds.Select(tid => Convert(_tripService.GetById(tid))).Cast<object>()
                    .Union(newsletter.HostIds.Select(hid => Convert(_hostService.GetById(hid))))
                    .ToList(),
            };

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
                Newsletters = _newsletterService.GetAll(),
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
            HostName = hostName ?? _hostService.GetById(trip.HostId)?.Name,
            ImageUrl = trip.ImageUrl,
        };
        
    }
}