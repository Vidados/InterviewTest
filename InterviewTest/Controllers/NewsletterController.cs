using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InterviewTest.Database;
using InterviewTest.Models;
using InterviewTest.Commands;
using InterviewTest.Services;

namespace InterviewTest.Controllers
{
    public class NewsletterController : Controller
    {
        private IDatabase _database;
        private ICommandHandler<CreateNewsletterCommand> _createNewsletterCommandHandler;

        public NewsletterController(
            IDatabase database,
            ICommandHandler<CreateNewsletterCommand> createNewsletterCommandHandler)
        {
            _database = database;
            _createNewsletterCommandHandler = createNewsletterCommandHandler;
        }

        public NewsletterController() : this(
            new FileSystemDatabase(),
            new CreateNewsletterCommandHandler(new FileSystemDatabase(), new NewsletterCompositionSpecificationParserService()))
        {
        }

        public ActionResult Create(int count, string specification)
        {
            _createNewsletterCommandHandler.Handle(new CreateNewsletterCommand
            {
                Count = count,
                Specification = specification
            });

            TempData["notification"] = $"Created {count} newsletters";

            return RedirectToAction("list");
        }

        public ActionResult DeleteAll()
        {
            _database.DeleteAll<Newsletter>();

            TempData["notification"] = "All newsletters deleted";

            return RedirectToAction("list");
        }

        public ActionResult Display(string id)
        {
            var newsletter = _database.Get<Newsletter>(id);

            var viewModel = new NewsletterViewModel
            {
                Items = (from item in newsletter.Items
                        from itemId in item.Ids                        
                        select item.Type == NewsletterItemType.Host
                            ? (object)Convert(_database.Get<Host>(itemId))
                            : Convert(_database.Get<Trip>(itemId))).ToList()
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
            var specification = (_database.GetAll<NewsletterCompositionSpecification>().FirstOrDefault()?.ToString()) ?? "HHHTTHHH";
            var viewModel = new NewsletterListViewModel
            {
                Specification = specification,
                Newsletters = _database.GetAll<Newsletter>(),
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
            HostName = hostName ?? _database.Get<Host>(trip.HostId)?.Name,
            ImageUrl = trip.ImageUrl,
        };

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
        public string Specification { get; set; }
        public List<Newsletter> Newsletters { get; set; }
    }
}