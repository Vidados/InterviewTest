using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Extensions;
using InterviewTest.Models;
using InterviewTest.Services;
using InterviewTest.ViewModels;

namespace InterviewTest.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly INewsletterService _newsletterService;
        private readonly IHostService _hostService;
        private readonly ITripService _tripService;

        public NewsletterController(INewsletterService newsletterService, IHostService hostService,
            ITripService tripService)
        {
            _newsletterService = newsletterService;
            _hostService = hostService;
            _tripService = tripService;
        }

        public ActionResult Create(int count, string pattern)
        {
            _newsletterService.CreateRandomData(count, pattern);
            TempData["notification"] = $"Created {count} newsletters in pattern {pattern}";
            
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

            if (newsletter == null)
                return RedirectToAction("list");

            var contents = _newsletterService.GetContents(newsletter);
            var viewModel = new NewsletterViewModel
            {
                Items = contents.Select(item => Convert(item)).ToList()
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

        private INewsletterItem Convert(IContent content, string hostName = null)
        {
            var host = content as Host;
            var trip = content as Trip;
            if (host != null)
            {
                return new NewsletterHostViewModel
                {
                    Name = host.Name,
                    ImageUrl = host.ImageUrl,
                    Job = host.Job,
                };
            }
            else if (trip != null)
            {
                return new NewsletterTripViewModel
                {
                    Name = trip.Name,
                    Country = trip.Country,
                    HostName = hostName ?? _hostService.GetById(trip.HostId)?.Name,
                    ImageUrl = trip.ImageUrl,
                };
            }
            return null;
        }

        //private NewsletterHostViewModel Convert(Host host) => new NewsletterHostViewModel
        //{
        //    Name = host.Name,
        //    ImageUrl = host.ImageUrl,
        //    Job = host.Job,
        //};

        //private NewsletterTripViewModel Convert(Trip trip, string hostName = null) => new NewsletterTripViewModel
        //{
        //    Name = trip.Name,
        //    Country = trip.Country,
        //    HostName = hostName ?? _hostService.GetById(trip.HostId)?.Name,
        //    ImageUrl = trip.ImageUrl,
        //};
    }
}
